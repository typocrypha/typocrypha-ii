public class BEConditionNumberOfReinforcements : BattleEventCondition
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
    
    public int num;
    public Operator op;

    public override bool Check()
    {
        int count = BattleManager.instance.CurrWave.reinforcementPrefabs.Count;
        if (op.HasFlag(Operator.EqualTo) && count == num)
            return true;
        if (op.HasFlag(Operator.GreaterThan) && count > num)
            return true;
        if (op.HasFlag(Operator.LessThan) && count < num)
            return true;
        return false;
    }
}
