using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CasterAbility : ScriptableObject
{
    public abstract void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target);
    public abstract void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, CastResults castResults);
    [System.Serializable] public class Set : SerializableSet<CasterAbility> { }
}
