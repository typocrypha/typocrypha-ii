using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BEConditionAllStatus : BattleEventCondition
{
    [SerializeField] private List<Caster.BattleStatus> statuses = new List<Caster.BattleStatus>();
    public override bool Check()
    {
        var enemies = Battlefield.instance.Enemies;
        return enemies.Count() > 0 && enemies.All(e => statuses.Contains(e.BStatus));
    }
}
