using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDoubleDamage : CasterAbility
{
    public SpellTag tag;

    public override void OnCast(Spell spell, RootWordEffect effect, Caster caster, Caster target)
    {
       if(effect.tags.Contains(tag) && effect is DamageEffect)
            (effect as DamageEffect).power *= 2;
    }

    public override void OnHit(Spell spell, RootWordEffect effect, Caster caster, Caster target, CastResults castResults)
    {
        return;
    }
}
