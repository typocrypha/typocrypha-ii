using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        target.BStatus = Caster.BattleStatus.Fled;
        return InitializeCastResults(caster, target, mod);
    }
}
