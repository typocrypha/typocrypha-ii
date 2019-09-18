using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BEConditionAllStatus : BattleEventCondition
{
    public List<Caster.BattleStatus> statuses = new List<Caster.BattleStatus>();
    public override bool Check()
    {
        bool ret = Battlefield.instance.Enemies.All((e) => statuses.Contains(e.BStatus));
        return ret;
    }
}
