using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormulaTazer : CustomFormula
{
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, bool crit, RootCastResults prevResults = null)
    {
        // Calculate standard results using the standard formula
        var results = Damage.Standard(effect, caster, target, crit);
        ++results.StaggerDamage;
        // Log some extra messages
        LogMessage(target.DisplayName + " is shocked!");
        // Apply the results of the cast in the standard manner
        Damage.ApplyStandard(results, effect, caster, target);
        return results;
    }
}
