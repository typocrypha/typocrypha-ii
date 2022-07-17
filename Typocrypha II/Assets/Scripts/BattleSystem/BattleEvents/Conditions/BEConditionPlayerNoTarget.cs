using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks an caster's health.
/// </summary>
public class BEConditionPlayerNoTarget : BattleEventCondition
{
    [SerializeField] private int numCasts;
    private int currCasts = 0;

    protected override void AddEventHandlers()
    {
        Battlefield.instance.Player.OnNoTargetHit += CheckCast;
    }

    protected override void RemoveEventHandlers()
    {
        Battlefield.instance.Player.OnNoTargetHit -= CheckCast;
    }

    public void CheckCast(Battlefield.Position pos)
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
