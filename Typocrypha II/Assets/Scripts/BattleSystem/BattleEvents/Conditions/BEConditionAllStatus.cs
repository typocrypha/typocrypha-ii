using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BEConditionAllStatus : BattleEventCondition
{
    [SerializeField] private List<Caster.BattleStatus> statuses = new List<Caster.BattleStatus>();
    public override bool Check()
    {
        bool foundEnemy = false;
        foreach(var caster in Battlefield.instance.Casters)
        {
            if (caster.CasterState != Caster.State.Hostile)
                continue;
            foundEnemy = true;
            if (!statuses.Contains(caster.BStatus))
                return false;
        }
        return foundEnemy;
    }
}
