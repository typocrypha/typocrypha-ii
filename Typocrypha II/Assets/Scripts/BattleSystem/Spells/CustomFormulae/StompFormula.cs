﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompFormula : CustomFormula
{
    public float damageBonusMod = 2; 
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, RootCastResults prevResults = null)
    {
        // Calculate standard results using the standard formula
        var results = Damage.Standard(effect, caster, target);
        if (target.HasTag("Short") || target.HasTag("Baby"))
        {
            results.Damage *= damageBonusMod;
            // Log some extra messages
            LogMessage(caster.DisplayName + " used a low blow!");
        }         
        // Apply the results of the cast in the standard manner
        Damage.ApplyStandard(results, effect, caster, target);
        return results;
    }
}
