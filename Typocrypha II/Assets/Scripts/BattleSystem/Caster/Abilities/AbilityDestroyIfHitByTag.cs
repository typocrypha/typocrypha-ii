using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDestroyIfHitByTag : CasterAbility
{
    public SpellTag tag;
    public Spell castOnDestroy;

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
        return;
    }

    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, CastResults castResults)
    {
        if (!effect.tags.Contains(tag) || castResults.Miss || !castResults.WillDealDamage || target.BStatus != Caster.BattleStatus.Normal)
        {
            return;
        }
        void DestroyCast() => target.CastImmediate(castOnDestroy, target.FieldPos);
        target.OnSpiritMode -= DestroyCast;
        target.OnSpiritMode += DestroyCast;
        castResults.Damage = target.Health * 2;
    }
}
