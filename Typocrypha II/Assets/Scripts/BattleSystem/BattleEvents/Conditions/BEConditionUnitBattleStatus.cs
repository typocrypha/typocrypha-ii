using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionUnitBattleStatus : BEConditionUnit
{
    [SerializeField] private List<Caster.BattleStatus> statuses = new List<Caster.BattleStatus>();
    protected override bool CheckCaster(Caster caster)
    {
        return statuses.Contains(caster.BStatus);
    }
}
