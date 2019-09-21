using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BEConditionNumberOfEnemies : BattleEventCondition
{
    [System.Flags]
    public enum Operator
    {
        None = 0,
        GreaterThan = 1,
        LessThan = 2,
        EqualTo = 4,
    }
    
    public int num;
    public Operator op;

    public override bool Check()
    {
        int count = Battlefield.instance.Enemies.Length;
        if (op.HasFlag(Operator.EqualTo) && count == num)
            return true;
        if (op.HasFlag(Operator.GreaterThan) && count > num)
            return true;
        if (op.HasFlag(Operator.LessThan) && count < num)
            return true;
        return false;
    }
}
