using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Check if the player has been hit by X damaging spells
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

    public void CheckCast(Spell s, Caster caster)
    {
        currCasts++;
        if (Check())
        {
            RemoveEventHandlers();
        }
    }

    public override bool Check()
    {
        return currCasts >= numCasts;
    }
}
