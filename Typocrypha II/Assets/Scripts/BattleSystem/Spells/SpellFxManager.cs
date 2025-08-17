using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpellManager))]
public class SpellFxManager : MonoBehaviour
{
    public const float popTime = 0.7f;
    private const float popTimeStaggered = popTime + staggerOffset;
    private const float staggerOffset = -0.6f;
    private const float minYieldTime = 0f;
    #region Damage Shake
    protected const float shakeIntensity = 0.125f;
    protected const float shakeDuration = 0.5f;
    protected const float shakeDamper = 5f;
    #endregion
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
    [SerializeField] private Transform spellResultsContainer;
    [SerializeField] private Transform textContainer;
    [Header("Effectiveness Sprites")]
    [SerializeField] private Sprite weakSprite = null;
    [SerializeField] private Sprite resistSprite = null;
    [SerializeField] private Sprite drainSprite = null;
    [SerializeField] private Sprite blockSprite = null;
    [SerializeField] private Sprite repelSprite = null;
    [SerializeField] private Sprite missSprite = null;
    [Header("Damage Fields")]
    [SerializeField] private MaterialController damageGlitchController;
    [Header("Log Fields")]
    [SerializeField] private BattleLog logger;
    [Header("Word Fx")]
    [SerializeField] private GameObject wordFxPrefab;
    [SerializeField] private Transform wordFxContainer;

    private PrefabPool<TextPopup> textPopupPool;
    private PrefabPool<TextPopup> damagePopupPool;
    private PrefabPool<ImagePopup> imagePopupPool;
    private PrefabPool<WordFx> wordFxPool;
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
        textPopupPool = new PrefabPool<TextPopup>(textPopupPrefab, textContainer, 10);
        damagePopupPool = new PrefabPool<TextPopup>(damagePopupPrefab, spellResultsContainer, 10);
        imagePopupPool = new PrefabPool<ImagePopup>(imagePopupPrefab, spellResultsContainer, 10);
        wordFxPool = new PrefabPool<WordFx>(wordFxPrefab, wordFxContainer, 10);
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
    public float NoTargetFx(Vector2 pos)
    {
        return PlayText(pos, true, "No Target", Color.red);
    }
    public float CounterFx(Battlefield.Position pos)
    {
        return PlayText(pos, "Countered!", Color.green, DisplayPopup.Animation.SlamIn, popTime + 0.1f);
    }
    public Coroutine Play(CastResults data, SpellWord word, Vector2 targetPos, Vector2 casterPos, System.Action onComplete = null)
    {
        // For some unknown reason, getting the animator within the coroutine instead of passing it in always gets null
        var targetAnim = data.target?.GetComponent<Animator>();
        return StartCoroutine(PlayCR(data, word, targetAnim, targetPos, casterPos, onComplete));
    }
    /// <summary> A coroutine to play multiple spell effects in a row to facilitate Modifier Fx with crList </summary>
    private IEnumerator PlayCR(CastResults data, SpellWord word, Animator targetAnim, Vector2 targetPos, Vector2 casterPos, System.Action onComplete)
    {
        bool playSpellAnimation = true;
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
            yield return new WaitForSeconds(PlayText(targetPos, true, "Miss", Color.white));
            onComplete?.Invoke();
            yield break;
        }
        #endregion

        if (word != null && data.WordFx != null)
        {
            var wordFx = wordFxPool.Get();
            bool completed = false;
            void Complete()
            {
                completed = true;
                wordFxPool.Release(wordFx);
            }
            wordFx.Play(word, data, Complete);
            yield return new WaitUntil(() => completed);
        }

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
            playSpellAnimation = false;
        }

        #endregion 

        if (playSpellAnimation)
        {
            foreach (var fx in data.AnimationData)
            {
                if (data.DisplayDamage && (data.WillDealDamage || data.Effectiveness == Reaction.Repel && data.Damage > 0))
                {
                    CameraManager.instance.Shake(shakeIntensity, shakeDuration, shakeDamper);
                    if (data.target.IsPlayer || (data.Effectiveness == Reaction.Repel && data.caster.IsPlayer))
                    {
                        damageGlitchController.Play();
                    }
                    else
                    {
                        data.target.transform.DOPunchScale(new Vector3(-0.1f, -0.133f), 0.5f, 0, 0);
                    }
                }
                yield return StartCoroutine(fx.Play(pos));
            }
        }
        float resultsWaitTime = PlayResultsPopup(data, pos, casterPos);
        if (resultsWaitTime > 0)
        {
            yield return new WaitForSeconds(resultsWaitTime);
        }
        onComplete?.Invoke();
    }

    #region Popup Effects

    public float PlayResultsPopup(CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        if (data == null)
            return 0;
        bool playEffect = false;
        // If damage should be displayed, display damage
        if (data.DisplayDamage && data.Effectiveness != Reaction.Block)
        {
            PlayDamageNumber(data.Damage, targetPos);
            playEffect = true;
        }
        // Effectiveness popup
        playEffect |= PlayReaction(data.Effectiveness, targetPos, casterPos) > 0;
        if (data.Stun)
        {
            PlayText(data.DisplayDamage ? targetPos + stunOffset : targetPos, true, "Stun!", Color.red);
            playEffect = true;
        }
        return playEffect ? popTimeStaggered : 0;
    }

    public float PlayDamageNumber(float damage, Caster target)
    {
        return PlayDamageNumber(damage, Battlefield.instance.GetSpaceScreenSpace(target.FieldPos));
    }

    public float PlayDamageNumber(float damage, Vector2 targetPos)
    {
        // If damage should be displayed, display damage
        var damageColor = damage < 0 ? Color.green : Color.white;
        var numberText = Mathf.FloorToInt(Mathf.Abs(damage)).ToString();
        var player = damagePopupPool.Get();
        player.transform.position = targetPos;
        player.Play(numberText, damageColor, popTime, DisplayPopup.Animation.FloatUp, damagePopupPool);
        return popTimeStaggered;
    }

    public float PlayReaction(Reaction reaction, Vector2 targetPos, Vector2 casterPos)
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
        return 0f;
    }

    private float PlayReaction(Sprite sprite, Vector2 targetPos)
    {
        return PlayImage(targetPos + reactionOffset, true, sprite, Color.white, popTime);
    }

    public float PlayText(Battlefield.Position pos, string text, Color color, DisplayPopup.Animation anim = DisplayPopup.Animation.FloatUp, float time = popTime)
    {
        return PlayText(Battlefield.instance.GetSpaceScreenSpace(pos), true, text, color, anim, time);
    }

    public float PlayText(Vector2 position, bool isScreenSpace, string text, Color color, DisplayPopup.Animation anim = DisplayPopup.Animation.FloatUp, float time = popTime)
    {
        var player = textPopupPool.Get();
        if (isScreenSpace)
        {
            player.transform.position = position;
        }
        else
        {
            player.transform.position = CameraManager.instance.Camera.WorldToScreenPoint(position);
        }
        player.Play(text, color, time, anim, textPopupPool);
        return Mathf.Max(minYieldTime, time + staggerOffset);
    }

    public float PlayImage(Vector2 position, bool isScreenSpace, Sprite image, Color color, float time)
    {
        var player = imagePopupPool.Get();
        if (isScreenSpace)
        {
            player.transform.position = position;
        }
        else
        {
            player.transform.position = CameraManager.instance.Camera.WorldToScreenPoint(position);
        }
        player.Play(image, color, time, DisplayPopup.Animation.FloatUp, imagePopupPool);
        return Mathf.Max(minYieldTime, time + staggerOffset);
    }

    #endregion

    private class LogData
    {
        public string text;
        public Sprite icon = null;
        public float? time;
    }
}
