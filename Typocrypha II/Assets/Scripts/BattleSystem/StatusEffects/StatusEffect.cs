using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    protected Caster affected;
    public CasterTag casterTag;

    private void Awake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    /// <summary>
    /// Initialize subscriptions to Caster callbacks
    /// </summary>
    protected virtual void Initialize()
    {
        affected = GetComponentInParent<Caster>();
        if (affected == null)
        {
            Debug.LogError("StatusEffect: " + name + " does could not find a caster on it's parent object ");
            return;
        }         
        affected.OnAfterHitResolved += OnAfterHit;
        affected.OnAfterSpellEffectResolved += OnAfterCastResolved;
        affected.OnBeforeHitResolved += OnBeforeAffectApplied;
    }

    public abstract void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data);

    public virtual void OnAfterHit(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {

    }

    public virtual void OnBeforeAffectApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {

    }

    public virtual void OnAfterCastResolved(Spell s, Caster self)
    {

    }

    /// <summary>
    /// Removes all subscriptions to Caster callbacks
    /// </summary>
    public virtual void Cleanup()
    {
        affected.OnAfterHitResolved -= OnAfterHit;
        affected.OnAfterSpellEffectResolved -= OnAfterCastResolved;
        affected.OnBeforeHitResolved -= OnBeforeAffectApplied;
    }

    /// <summary>
    /// Simply removes the associated tag from the caster.
    /// Cleanup and actual destruction are called in CasterTagDictionary when the associated tag is removed
    /// </summary>
    public virtual void Remove()
    {
        affected.RemoveTag(casterTag);
    }
}
