﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    protected Caster affected;
    public CasterTag casterTag;

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
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
        affected.OnAfterCastResolved += OnAfterCastResolved;
    }

    public virtual void OnAfterHit(RootWordEffect effect, Caster caster, Caster target, CastResults data)
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
        affected.OnAfterCastResolved -= OnAfterCastResolved;
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
