using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDoubleDamage : CasterAbility
{
    [SerializeField] SpellTag tag;

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
       if(effect.tags.Contains(tag) && effect is DamageEffect)
            (effect as DamageEffect).power *= 2;
    }

    public override void OnHit(RootWordEffect effect, Caster caster, Caster target, CastResults castResults)
    {
        return;
    }
}
