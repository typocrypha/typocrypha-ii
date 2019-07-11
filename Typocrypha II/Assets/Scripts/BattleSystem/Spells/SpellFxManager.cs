using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpellManager))]
public class SpellFxManager : MonoBehaviour
{
    private const float popTime = 0.5f;
    private const float logTime = 0.8f;
    public static SpellFxManager instance;
    public bool HasMessages { get => logData.Count > 0; }
    [Header("No Target FX")]
    [SerializeField] private SpellFxData noTargetFx = new SpellFxData();
    [Header("Repel FX")]
    [SerializeField] private SpellFxData repelFx = new SpellFxData();
    [Header("Default Popup Prefab")]
    [SerializeField] private GameObject popupPrefab = null;
    [Header("Effectiveness Sprites")]
    [SerializeField] private Sprite weakSprite = null;
    [Header("Log Fields")]
    [SerializeField] private Vector2 logPosition = new Vector2(0.5f, 0.5f);
    [SerializeField] private Sprite logImage = null;

    private Queue<LogData> logData = new Queue<LogData>();
    /// <summary> Singleton implementation </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
            var popper = Instantiate(message.popupPrefab).GetComponent<PopupBase>();
            var cr1 = popper.PopImage(message.image ?? logImage, logPosition, logTime);
            var cr2 = popper.PopText(message.text, logPosition, logTime);
            yield return cr1;
            yield return cr2;
            popper.Cleanup();
        }
    }
    public void LogMessage(string message, Sprite image = null, GameObject popupPrefabOverride = null)
    {
        logData.Enqueue(new LogData() { text = message, image = image, popupPrefab = popupPrefabOverride ?? popupPrefab });
    }
    public Coroutine NoTargetFx(Vector2 pos)
    {
        return StartCoroutine(noTargetFx.Play(pos));
    }
    public Coroutine Play(SpellFxData[] fxData, CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        return StartCoroutine(PlayCR(fxData, data, targetPos, casterPos));
    }
    /// <summary> A coroutine to play multiple spell effects in a row to facilitate Modifier Fx with crList </summary>
    private IEnumerator PlayCR(SpellFxData[] fxData, CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        var pos = targetPos;

        #region Repel
        if (data.Effectiveness == Reaction.Repel)
        {
            pos = casterPos;
            yield return StartCoroutine(repelFx.Play(targetPos));
        }
        #endregion

        foreach (var fx in fxData)
            if (fx != null)
                yield return StartCoroutine(fx.Play(pos));

        yield return StartCoroutine(PlayPopupCr(data, pos, casterPos));
    }

    #region Popup Effects

    public Coroutine PlayPopup(CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        return StartCoroutine(PlayPopupCr(data, targetPos, casterPos));
    }
    private IEnumerator PlayPopupCr(CastResults data, Vector2 targetPos, Vector2 casterPos)
    {
        if (data == null)
            yield break;
        var popper = Instantiate(data.popupPrefab ?? popupPrefab).GetComponent<PopupBase>();

        yield return popper.PopText(data.Damage.ToString(), targetPos, popTime);
        switch (data.Effectiveness)
        {
            case Reaction.Weak:
                yield return popper.PopImage(weakSprite, targetPos, popTime);
                break;
            case Reaction.Neutral:
                break;
            case Reaction.Resist:
                break;
            case Reaction.Block:
                break;
            case Reaction.Dodge:
                break;
            case Reaction.Drain:
                break;
            case Reaction.Repel:
                break;
        }
        popper.Cleanup();
    }

    #endregion

    private class LogData
    {
        public string text;
        public Sprite image = null;
        public GameObject popupPrefab;
    }
}
