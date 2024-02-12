using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class BattleDimmer : MonoBehaviour
{
    public static BattleDimmer instance;
    public Color inactiveColor = new Color(0, 0, 0, 0.0f);
    public Color activeColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] SpriteRenderer dimmerSprite;
    [SerializeField] TweenInfo info;

    private readonly List<CasterUI> dimmedUIs = new List<CasterUI>();
    private bool active = false;
    private Tween dimTween;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Sets the active state of the dimmer.
    /// When active, the battle background and any specified casters will be affected by the dimming layer.
    /// When deactivating, all casters will remain in a dimmable state until the animation completes.
    /// </summary>
    /// <param name="active">Whether to active or deactivate the dimmer.</param>
    public void SetDimmer(bool active)
    {
        if (active != this.active)
        {
            if (dimTween != null && dimTween.IsActive() && dimTween.IsPlaying()) dimTween.Complete();
            dimTween = dimmerSprite.DOColor(active ? activeColor : inactiveColor, info.Time);
            if (!active) dimTween.OnComplete(UndimAllCasters); //complete animation before resetting sorting order
            this.active = active;
        }
    }

    /// <summary>
    /// Enable dimming effect on specified casters.
    /// </summary>
    /// <param name="casters">A collection of casters to make dimmable.</param>
    /// <param name="showUI">Whether to hide caster UI.</param>
    public void DimCasters(IEnumerable<Caster> casters, bool showUI = true)
    {
        if (casters == null) return;

        foreach (var c in casters)
        {
            if (!c || !c.ui) continue;
            dimmedUIs.Add(c.ui);
            c.ui.SetDimmable(true).ShowUI(showUI);
        }
    }

    /// <summary>
    /// Disabler dimming effect on specified casters.
    /// </summary>
    /// <param name="casters">A collection of casters to make undimmable.</param>
    public void UndimCasters(IEnumerable<Caster> casters)
    {
        if (casters == null) return;

        foreach (var c in casters)
        {
            if (!c || !c.ui) continue;
            c.ui.SetDimmable(false).ShowUI(true);
            dimmedUIs.Remove(c.ui);
        }
    }

    /// <summary>
    /// Disable dimming effect on all casters.
    /// </summary>
    public void UndimAllCasters()
    {
        foreach (var ui in dimmedUIs)
        {
            if (!ui) continue;
            ui.SetDimmable(false).ShowUI(true);
        }
        dimmedUIs.Clear();
    }
}