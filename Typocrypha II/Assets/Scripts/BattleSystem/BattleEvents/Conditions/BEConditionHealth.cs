using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks an caster's health.
/// </summary>
public class BEConditionHealth : BattleEventCondition
{
    public string targetName; // Name of caster whose health you want to check.
    public int threshold; // Health threshold to check.
    Caster targetCaster; // Caster whose health we're checking.

    void Start()
    {
        foreach (var caster in Battlefield.instance.Casters)
        {
            if (caster.DisplayName == targetName)
            {
                targetCaster = caster;
                break;
            }
        }
    }

    public override bool Check()
    {
        return targetCaster.Health <= threshold;
    }
}
