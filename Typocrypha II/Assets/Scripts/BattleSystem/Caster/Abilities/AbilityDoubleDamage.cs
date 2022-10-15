using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDoubleDamage : CasterAbility
{
    [SerializeField] SpellTag tag;

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
       if(effect.tags.Contains(tag) && effect is DamageEffect damageEffect)
            damageEffect.power *= 2;
    }

    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        return;
    }
}
