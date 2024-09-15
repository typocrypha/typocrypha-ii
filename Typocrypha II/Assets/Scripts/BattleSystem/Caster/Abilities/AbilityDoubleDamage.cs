using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDoubleDamage : CasterAbility
{
    [SerializeField] SpellTag tag;

    public override void AddTo(Caster caster)
    {
        caster.OnBeforeSpellEffectCast += OnBeforeSpellEffectCast;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.OnBeforeSpellEffectCast -= OnBeforeSpellEffectCast;
    }

    public void OnBeforeSpellEffectCast(RootWordEffect effect, Caster caster, Caster target, Damage.DamageModifier mod)
    {
       if(effect.tags.Contains(tag) && effect is DamageEffect damageEffect)
            damageEffect.power *= 2;
    }
}
