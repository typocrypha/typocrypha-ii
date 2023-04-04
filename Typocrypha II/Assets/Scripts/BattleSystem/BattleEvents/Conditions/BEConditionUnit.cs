using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BEConditionUnit : BattleEventCondition
{
    public enum Operator 
    {
        Any,
        All,
        SpecificName,
        Player,
    }

    [SerializeField] Operator op;
    [SerializeField] private Caster.State casterState = Caster.State.Hostile;
    [SerializeField] private string specificName = "";

    public override bool Check()
    {
        if(op == Operator.Player)
        {
            return CheckCaster(Battlefield.instance.Player);
        }
        foreach(var caster in Battlefield.instance.Casters)
        {
            if (caster.CasterState != casterState)
                continue;
            if (op == Operator.SpecificName)
            {
                if(caster.DisplayName.ToLower() == specificName.ToLower())
                {
                    return CheckCaster(caster);
                }
            }
            else if(op == Operator.Any)
            {
                if (CheckCaster(caster))
                    return true;
            }
            else // All operator
            {
                if (!CheckCaster(caster))
                    return false;
            }
        }
        return op == Operator.All;
    }

    protected abstract bool CheckCaster(Caster caster);
}
