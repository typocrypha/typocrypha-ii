using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBrittle : CasterAbility
{
    public override void AddTo(Caster caster)
    {
        caster.OnBeforeHitResolved += OnBeforeHitResolved;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.OnBeforeHitResolved -= OnBeforeHitResolved;
    }

    public void OnBeforeHitResolved(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if(spellData.IsLastRoot && castResults.Combo >= 2 && !target.Stunned)
        {
            castResults.StaggerDamage += 1;
            SpellFxManager.instance.LogMessage($"The multicast staggered {target.DisplayName}!", null, 1f);
        }
    }
}
