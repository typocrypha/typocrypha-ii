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

    private readonly List<Caster> dimmedCasters = new List<Caster>();
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
    public void DimCasters(IEnumerable<Caster> casters)
    {
        if (casters == null) return;

        foreach (var caster in casters)
        {
            if (!caster || !caster.ui) 
                continue;
            dimmedCasters.Add(caster);
            caster.ui.SetDimmable(true);
        }
    }

    /// <summary>
    /// Enable dimming effect on specified casters.
    /// </summary>
    /// <param name="casters">A collection of casters to make dimmable.</param>
    public void DimCasters(IEnumerable<Caster> casters, Caster except)
    {
        if (casters == null) return;

        foreach (var caster in casters)
        {
            if (!caster || !caster.ui || caster == except) 
                continue;
            dimmedCasters.Add(caster);
            caster.ui.SetDimmable(true);
        }
    }

    public void UndimCastersAtPositions(IEnumerable<Battlefield.Position> positions)
    {
        foreach (var pos in positions)
        {
            var caster = Battlefield.instance.GetCaster(pos);
            if (caster == null)
                continue;
            UndimCaster(caster);
        }
    }

    public void UndimCaster(Caster caster)
    {
        if (!caster || !caster.ui) return;
        caster.ui.SetDimmable(false);
        caster.ShowUI(true);
        dimmedCasters.Remove(caster);
    }

    /// <summary>
    /// Disable dimming effect on all casters.
    /// </summary>
    public void UndimAllCasters()
    {
        foreach (var caster in dimmedCasters)
        {
            if (caster == null || caster.ui == null) 
                continue;
            caster.ui.SetDimmable(false);
            caster.ShowUI(true);
        }
        dimmedCasters.Clear();
    }
}