using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDoubleDamage : CasterAbility
{
    [SerializeField] SpellTag tag;

    public override void AddTo(Caster caster)
    {
        caster.OnBeforeSpellEffectResolved += OnBeforeSpellEffectResolved;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.OnBeforeSpellEffectResolved -= OnBeforeSpellEffectResolved;
    }

    public void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
       if(effect.tags.Contains(tag) && effect is DamageEffect damageEffect)
            damageEffect.power *= 2;
    }
}
