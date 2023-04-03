using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Check if the player has cast X times
/// </summary>
public class BEConditionPlayerMultiCast : BEConditionPlayerCast
{
    [SerializeField] private BEConditionNumberComparison.Operator op;
    [SerializeField] private int numRoots;
    protected override bool IsValidCast(Spell s, Caster caster)
    {
        return base.IsValidCast(s, caster) && BEConditionNumberComparison.EvalComparison(numRoots, s.RootCount, op);
    }
}
