using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CasterAbility : ScriptableObject
{
    public virtual void AddTo(Caster caster)
    {
        caster.OnBeforeHitResolved += OnBeforeHitApplied;
        caster.OnBeforeSpellEffectResolved += OnBeforeSpellEffectResolved;
    }
    public virtual void RemoveFrom(Caster caster)
    {
        caster.OnBeforeHitResolved -= OnBeforeHitApplied;
        caster.OnBeforeSpellEffectResolved -= OnBeforeSpellEffectResolved;
    }
    public abstract void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target);
    public abstract void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults);
    [System.Serializable] public class Set : SerializableSet<CasterAbility> { }
}
