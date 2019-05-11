using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LuckDamage", menuName = "Formulae/LuckDamage")]
public class FomulaLuckDmg : CustomFormula
{
    public float scalingFactor = 0.33f; 
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target)
    {
        var results = Damage.StandardElements(effect, caster, target);
        float luckfactor = 1 + ((caster.Stats.Luck + 1) - target.Stats.Luck) * scalingFactor;
        results.damage = Mathf.Floor(results.damage * effect.power * luckfactor);
        target.Health -= (int)results.damage;
        return results;
    }
}
