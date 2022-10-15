using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormulaRequital : CustomFormula
{
    public CasterTag markTag;
    public float intensityMultiplier = 1;
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, bool crit, RootCastData spellData, RootCastResults prevResults = null)
    {
        // Calculate standard results using the standard formula
        var results = Damage.Standard(effect, caster, target, crit, spellData);
        // Extra marking effects
        if(target.HasTag(markTag))
        {
            // Remove the mark if the attack is a miss
            if (results.Miss)
            {
                target.RemoveTag(markTag);
            }
            else // Add extra mark damage
            {
                var mark = target.GetStatusEffect(markTag) as StatusMarked;
                if (mark != null)
                {
                    results.Damage += mark.intensity * intensityMultiplier;
                }
            }
        }
        // Apply the results of the cast in the standard manner
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
