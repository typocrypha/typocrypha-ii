using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Check if the player has cast X times
/// </summary>
public class BEConditionPlayerCast : BattleEventCondition
{
    [SerializeField] private int numCasts;
    private int currCasts = 0;

    protected override void AddEventHandlers()
    {
        Battlefield.instance.Player.OnAfterCastResolved += CheckCast;
    }

    protected override void RemoveEventHandlers()
    {
        Battlefield.instance.Player.OnAfterCastResolved -= CheckCast;
    }

    public void CheckCast(Spell s, Caster caster, bool hitTarget)
    {
        if (!IsValidCast(s, caster))
            return;
        currCasts++;
        if (Check())
        {
            RemoveEventHandlers();
        }
    }

    protected virtual bool IsValidCast(Spell s, Caster caster)
    {
        return true;
    }

    public override bool Check()
    {
        return currCasts >= numCasts;
    }
}
