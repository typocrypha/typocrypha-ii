using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Always returns false
/// </summary>
public class BEConditionFalse : BattleEventCondition
{
    protected override bool CheckInternal()
    {
        return false;
    }
}
