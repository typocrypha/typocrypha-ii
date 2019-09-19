using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks an caster's health.
/// </summary>
public class BEConditionPlayerNoTarget : BattleEventCondition
{
    public int numCasts;
    private int currCasts = 0;

    void Start()
    {
        Battlefield.instance.Player.OnNoTargetHit += CheckCast;
    }

    private void OnDestroy()
    {
        if (currCasts < numCasts)
            Battlefield.instance.Player.OnNoTargetHit -= CheckCast;
    }

    public void CheckCast(Battlefield.Position pos)
    {
        currCasts++;
        if (currCasts >= numCasts)
            Battlefield.instance.Player.OnNoTargetHit -= CheckCast;
    }

    public override bool Check()
    {
        return currCasts >= numCasts;
    }
}
