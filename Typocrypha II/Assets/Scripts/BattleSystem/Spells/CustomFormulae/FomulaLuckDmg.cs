using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LuckDamage", menuName = "Formulae/LuckDamage")]
public class FomulaLuckDmg : CustomFormula
{
    public float scalingFactor = 0.33f; 
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target)
    {
        // Calculate standard results using the standard formula
        var results = Damage.Standard(effect, caster, target);
        // Calculate the luck factor and multiply damage by it
        float luckfactor = 1 + ((caster.Stats.Luck + 1) - target.Stats.Luck) * scalingFactor;
        results.Damage *= luckfactor;
        // Apply the results of the cast in the standard manner
        Damage.ApplyStandard(results, effect, caster, target);
        // Log some extra messages
        LogMessage(caster.DisplayName + " is feeling lucky!");
        return results;
    }
}
