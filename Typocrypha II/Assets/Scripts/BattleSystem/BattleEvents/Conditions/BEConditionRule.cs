using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionRule : BattleEventCondition
{
    [SerializeField] private string rule;
    protected override bool CheckInternal()
    {
        return Rule.ActiveRule != null && Rule.ActiveRule.DisplayName.ToLower() == rule.ToLower();
    }
}
