using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBrittle : CasterAbility
{
    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if(spellData.IsLastRoot && castResults.Combo >= 2)
        {
            castResults.StaggerDamage += 1;
            SpellFxManager.instance.LogMessage($"{target.DisplayName} was shattered!");
        }
    }

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
        return;
    }
}
