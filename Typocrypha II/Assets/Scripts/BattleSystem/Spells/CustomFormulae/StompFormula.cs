using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompFormula : CustomFormula
{
    public float damageBonusMod = 2; 
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.DamageModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        // Calculate standard results using the standard formula
        var results = Damage.Standard(effect, caster, target, mod, spellData);
        if (target.HasTag("Short") || target.HasTag("Baby"))
        {
            results.Damage *= damageBonusMod;
            // Log some extra messages
            LogMessage(target.DisplayName + " was crushed!");
        }
        // Apply the results of the cast in the standard manner
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
