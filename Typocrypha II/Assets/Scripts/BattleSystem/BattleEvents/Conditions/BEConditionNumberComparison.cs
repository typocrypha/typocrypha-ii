using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BEConditionNumberComparison : BattleEventCondition
{
    [System.Flags]
    public enum Operator
    {
        None = 0,
        GreaterThan = 1,
        LessThan = 2,
        EqualTo = 4,
        LessThanOrEqual = LessThan | EqualTo,
        GreaterThanOrEqual = GreaterThan | EqualTo,
    }

    [SerializeField] private int num;
    [SerializeField] private Operator op;

    public override bool Check()
    {
        int count = Number;
        if (op.HasFlag(Operator.EqualTo) && count == num)
            return true;
        if (op.HasFlag(Operator.GreaterThan) && count > num)
            return true;
        return op.HasFlag(Operator.LessThan) && count < num;
    }

    protected abstract int Number { get; }
}
