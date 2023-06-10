using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpellManager))]
public class SpellFxManager : MonoBehaviour
{
    public const float popTime = 0.4f;
    public static SpellFxManager instance;
    private static readonly Vector2 reactionOffset = new Vector2(0, -80);
    private static readonly Vector2 stunOffset = new Vector2(0, 80);
    public bool HasMessages { get => logData.Count > 0; }

    [Header("Repel FX")]
    [SerializeField] private SpellFxData repelFx = new SpellFxData();
    [Header("Drain FX")]
    [SerializeField] private SpellFxData drainFx = new SpellFxData();
    [Header("Block FX")]
    [SerializeField] private SpellFxData blockFx = new SpellFxData();
    [Header("Popup Fields")]
    [SerializeField] private GameObject textPopupPrefab;
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private GameObject imagePopupPrefab;
    [SerializeField] private Canvas popupCanvas;
    [Header("Effectiveness Sprites")]
    [SerializeField] private Sprite weakSprite = null;
    [SerializeField] private Sprite resistSprite = null;
    [SerializeField] private Sprite drainSprite = null;
    [SerializeField] private Sprite blockSprite = null;
    [SerializeField] private Sprite repelSprite = null;
    [SerializeField] private Sprite missSprite = null;
    [Header("Log Fields")]
    [SerializeField] private BattleLog logger;

    private PrefabPool<TextPopup> textPopupPool;
    private PrefabPool<TextPopup> damagePopupPool;
    private PrefabPool<ImagePopup> imagePopupPool;
    private Queue<LogData> logData = new Queue<LogData>();
    /// <summary> Singleton implementation </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        textPopupPool = new PrefabPool<TextPopup>(textPopupPrefab, 10);
        damagePopupPool = new PrefabPool<TextPopup>(damagePopupPrefab, 10);
        imagePopupPool = new PrefabPool<ImagePopup>(imagePopupPrefab, 10);
    }

    public Coroutine PlayMessages()
    {
        return StartCoroutine(PlayMessagesCr());
    }
    private IEnumerator PlayMessagesCr()
    {
        while (logData.Count > 0)
        {
            var message = logData.Dequeue();
            logger.SetContent(message.text, message.icon, message.time);
            logger.gameObject.SetActive(true);
            yield return logger.Play();
            logger.gameObject.SetActive(false);
        }
    }
    public void LogMessage(string message, Sprite icon = null, float? time = null)
    {
        logData.Enqueue(new LogData() { text = message, icon = icon, time = time});
    }
    public Coroutine NoTargetFx(Vector2 pos)
    {
        return PlayText(pos, true, "No Target", Color.red, popTime);
    }
    public Coroutine Play(SpellFxData[] fxData, CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        // For some unknown reason, getting the animator within the coroutine instead of passing it in always gets null
        var targetAnim = data.target?.GetComponent<Animator>();
        return StartCoroutine(PlayCR(fxData, targetAnim, data, targetPos, casterPos));
    }
    /// <summary> A coroutine to play multiple spell effects in a row to facilitate Modifier Fx with crList </summary>
    private IEnumerator PlayCR(SpellFxData[] fxData, Animator targetAnim, CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        var pos = targetPos;

        #region Miss
        if(data.Miss)
        {
            if (targetAnim != null && targetAnim.HasState(0, Animator.StringToHash("Dodge")))
            {
                targetAnim.Play("Dodge");
                yield return new WaitForSeconds(0.25f);
                targetAnim.SetTrigger("Idle");
            }
            yield return PlayText(targetPos, true, "Miss", Color.white, popTime);
            yield break;
        }
        #endregion

        #region Special Reaction Graphics

        //Repel
        if (data.Effectiveness == Reaction.Repel)
        {
            pos = casterPos;
            yield return StartCoroutine(repelFx.Play(targetPos));
        }
        else if (data.Effectiveness == Reaction.Drain)
        {
            yield return StartCoroutine(drainFx.Play(targetPos));
        }
        else if (data.Effectiveness == Reaction.Dodge)
        {
            if (targetAnim != null && targetAnim.HasState(0, Animator.StringToHash("EnemyDodge")))
            {
                targetAnim.SetTrigger("Dodge");
                yield return new WaitForSeconds(0.33f);
            }
        }
        else if (data.Effectiveness == Reaction.Block)
        {
            yield return StartCoroutine(blockFx.Play(targetPos));
        }

        #endregion 

        foreach (var fx in fxData)
        {
            if (fx != null)
            {
                yield return StartCoroutine(fx.Play(pos));
            }
        }
        yield return StartCoroutine(PlayPopupCr(data, pos, casterPos));
    }

    #region Popup Effects

    public Coroutine PlayFullPopup(CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        return StartCoroutine(PlayPopupCr(data, targetPos, casterPos));
    }
    private IEnumerator PlayPopupCr(CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        if (data == null)
            yield break;
        Coroutine damageRoutine = null; 
        // If damage should be displayed, display damage
        if(data.DisplayDamage)
        {
            damageRoutine = PlayDamageNumber(data.Damage, targetPos);
        }
        // Effectiveness popup
        Coroutine reactionRoutine = PlayReaction(data.Effectiveness, targetPos, casterPos);
        Coroutine stunRoutine = null;
        if (data.Stun)
        {
            stunRoutine = PlayText(data.DisplayDamage ? targetPos + stunOffset : targetPos, true, "Stun!", Color.red, popTime);
        }
        if(stunRoutine != null)
        {
            yield return stunRoutine;
        }
        if(damageRoutine != null)
        {
            yield return damageRoutine;
        }
        if(reactionRoutine != null)
        {
            yield return reactionRoutine;
        }
    }

    public Coroutine PlayDamageNumber(float damage, Vector2 targetPos)
    {
        // If damage should be displayed, display damage
        var damageColor = damage < 0 ? Color.green : Color.white;
        var numberText = Mathf.FloorToInt(Mathf.Abs(damage)).ToString();
        var player = damagePopupPool.Get(popupCanvas.transform);
        player.transform.position = targetPos;
        return player.Play(numberText, damageColor, popTime, damagePopupPool);
    }

    public Coroutine PlayReaction(Reaction reaction, Vector2 targetPos, Vector2 casterPos)
    {
        switch (reaction)
        {
            case Reaction.Weak:
                return PlayReaction(weakSprite, targetPos);
            case Reaction.Neutral:
                break;
            case Reaction.Resist:
                return PlayReaction(resistSprite, targetPos);
            case Reaction.Block:
                return PlayReaction(blockSprite, targetPos);
            case Reaction.Dodge:
                return PlayReaction(missSprite, targetPos);
            case Reaction.Drain:
                return PlayReaction(drainSprite, targetPos);
            case Reaction.Repel:
                return PlayReaction(repelSprite, targetPos);
        } 
        return null;
    }

    private Coroutine PlayReaction(Sprite sprite, Vector2 targetPos)
    {
        return PlayImage(targetPos + reactionOffset, true, sprite, Color.white, popTime);
    }

    public Coroutine PlayText(Vector2 position, bool isScreenSpace, string text, Color color, float time)
    {
        var player = textPopupPool.Get(popupCanvas.transform);
        if (isScreenSpace)
        {
            player.transform.position = position;
        }
        else
        {
            player.transform.position = CameraManager.instance.Camera.WorldToScreenPoint(position);
        }
        return player.Play(text, color, time, textPopupPool);
    }

    public Coroutine PlayImage(Vector2 position, bool isScreenSpace, Sprite image, Color color, float time)
    {
        var player = imagePopupPool.Get(popupCanvas.transform);
        if (isScreenSpace)
        {
            player.transform.position = position;
        }
        else
        {
            player.transform.position = CameraManager.instance.Camera.WorldToScreenPoint(position);
        }
        return player.Play(image, color, time, imagePopupPool);
    }

    #endregion

    private class LogData
    {
        public string text;
        public Sprite icon = null;
        public float? time;
    }
}
