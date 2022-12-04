using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FomulaLuckDmg : CustomFormula
{
    public float scalingFactor = 0.33f; 
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.SpecialModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        // Calculate standard results using the standard formula
        var results = Damage.Standard(effect, caster, target, mod, spellData);
        // Calculate the luck factor and multiply damage by it
        float luckfactor = Damage.CompareStats(caster.Stats.Luck, target.Stats.Luck) * scalingFactor;
        results.Damage *= luckfactor;
        // Log some extra messages
        LogMessage(caster.DisplayName + " is feeling lucky!");
        // Apply the results of the cast in the standard manner
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
