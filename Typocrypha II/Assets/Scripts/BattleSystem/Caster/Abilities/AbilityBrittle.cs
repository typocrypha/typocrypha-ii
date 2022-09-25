using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBrittle : CasterAbility
{
    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, CastResults castResults)
    {
        if(castResults.Combo >= 2)
        {
            castResults.StaggerDamage += 1;
        }
    }

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
        return;
    }
}
