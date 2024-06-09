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

    public abstract string FailMessage(Caster caster, Caster target);

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
    }

    public virtual void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data) { }

    /// <summary>
    /// Removes all subscriptions to Caster callbacks
    /// </summary>
    public virtual void Cleanup() { }

    /// <summary>
    /// Simply removes the associated tag from the caster.
    /// Cleanup and actual destruction are called in CasterTagDictionary when the associated tag is removed
    /// </summary>
    public virtual void Remove()
    {
        affected.RemoveTag(casterTag);
    }
}
