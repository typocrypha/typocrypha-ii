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

    public override void OnHit(RootWordEffect effect, Caster caster, Caster target, CastResults castResults)
    {
        if (!effect.tags.Contains(tag))
        {
            return;
        }
        target.OnSpiritMode += () => SpellManager.instance.Cast(castOnDestroy, target, target.FieldPos);
        target.Damage(1000);
    }
}
