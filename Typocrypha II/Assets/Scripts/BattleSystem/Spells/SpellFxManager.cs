using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpellManager))]
public class SpellFxManager : MonoBehaviour
{
    public static SpellFxManager instance;
    [Header("No Target FX")]
    [SerializeField] private SpellFxData noTargetFx = new SpellFxData();
    [Header("Repel FX")]
    [SerializeField] private SpellFxData repelFx = new SpellFxData();
    [Header("Default Popup Prefab")]
    [SerializeField] private GameObject popupPrefab;
    [Header("Effectiveness Sprites")]
    [SerializeField] private Sprite weakSprite;

    /// <summary> Singleton implementation </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
        if (data.effectiveness == Reaction.Repel)
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

        popper.PopText(data.damage.ToString(), targetPos, 0.75f);
        switch (data.effectiveness)
        {
            case Reaction.Weak:
                popper.PopImage(weakSprite, targetPos, 0.75f);
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
        yield return null;
    }

    #endregion
}
