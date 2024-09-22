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

    public static bool EvalComparison(int number, int currentValue, Operator op)
    {
        if (op.HasFlag(Operator.EqualTo) && currentValue == number)
            return true;
        if (op.HasFlag(Operator.GreaterThan) && currentValue > number)
            return true;
        return op.HasFlag(Operator.LessThan) && currentValue < number;
    }

    [SerializeField] private int num;
    [SerializeField] private Operator op;

    protected override bool CheckInternal()
    {
        return EvalComparison(num, Number, op);
    }

    protected abstract int Number { get; }
}
