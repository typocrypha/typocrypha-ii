using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BEConditionAllKO : BattleEventCondition
{
    public override bool Check()
    {
        bool ret = Battlefield.instance.Enemies.All((e) => e.Health <= 0);
        return ret;
    }
}
