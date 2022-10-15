using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunEffect : RootWordEffect
{
    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, bool crit, RootCastData spellData, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target);
        results.Miss = false;
        results.DisplayDamage = false;
        target.BStatus = Caster.BattleStatus.Fled;
        return results;
    }
}
