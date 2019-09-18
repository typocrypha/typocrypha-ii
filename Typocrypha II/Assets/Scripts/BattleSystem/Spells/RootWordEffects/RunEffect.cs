using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target);
        results.Miss = false;
        results.DisplayDamage = false;
        target.BStatus = Caster.BattleStatus.Fled;
        return results;
    }
}
