using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityIncomingDamageMultiplier : CasterAbility
{
    public override void AddTo(Caster caster)
    {
        caster.OnBeforeHitResolved += OnBeforeHitResolved;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.OnBeforeHitResolved -= OnBeforeHitResolved;
    }

    public void OnBeforeHitResolved(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if (!castResults.WillDealDamage || !ShouldApplyAbility(effect, caster, target, castResults))
            return;
        castResults.Damage *= Multiplier;
    }

    protected abstract float Multiplier { get; }
    protected abstract bool ShouldApplyAbility(RootWordEffect effect, Caster caster, Caster self, CastResults castResults);
}
