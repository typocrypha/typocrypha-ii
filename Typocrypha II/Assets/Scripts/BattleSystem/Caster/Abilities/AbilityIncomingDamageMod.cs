using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityIncomingDamageMod : CasterAbility
{
    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if (!castResults.WillDealDamage || !ShouldApplyAbility(effect, caster, target, castResults))
            return;
        castResults.Damage *= Multiplier;
    }

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
        return;
    }
    protected abstract float Multiplier { get; }
    protected abstract bool ShouldApplyAbility(RootWordEffect effect, Caster caster, Caster self, CastResults castResults);
}
