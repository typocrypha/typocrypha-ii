using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBrittle : CasterAbility
{
    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if(spellData.IsLastRoot && castResults.Combo >= 2 && !target.Stunned)
        {
            castResults.StaggerDamage += 1;
            SpellFxManager.instance.LogMessage($"The multicast staggered {target.DisplayName}!", null, 1f);
        }
    }

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
        return;
    }
}
