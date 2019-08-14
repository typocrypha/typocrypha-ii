using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    protected Caster affected;
    public CasterTag casterTag;

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        affected = GetComponentInParent<Caster>();
        if (affected == null)
        {
            Debug.LogError("StatusEffect: " + name + " does could not find a caster on it's parent object ");
            return;
        }         
        affected.OnAfterHitResolved += OnAfterHit;
        affected.OnAfterCastResolved += OnAfterCastResolved;
    }

    public virtual void OnAfterHit(RootWordEffect effect, Caster caster, Caster target, CastResults data)
    {

    }

    public virtual void OnAfterCastResolved(Spell s, Caster self)
    {

    }

    public virtual void CleanupAndDestroy()
    {
        affected.OnAfterHitResolved -= OnAfterHit;
        affected.OnAfterCastResolved -= OnAfterCastResolved;
        affected.RemoveTag(casterTag);
    }
}
