using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks when caster casts spell certain number of times
/// </summary>
public class BEConditionPlayerCast : BattleEventCondition
{
    public string casterName; // Name of caster whose health you want to check.
    public int numCasts;
    private int currCasts = 0;

    void Start()
    {
        Battlefield.instance.Player.OnBeforeHitResolved += CheckCast;
    }

    private void OnDestroy()
    {
        if (currCasts < numCasts)
            Battlefield.instance.Player.OnBeforeHitResolved -= CheckCast;
    }

    public void CheckCast(RootWordEffect effect,Caster caster,Caster target, CastResults data)
    {
        currCasts++;
        if (caster.DisplayName == casterName && currCasts >= numCasts)
            Battlefield.instance.Player.OnBeforeHitResolved -= CheckCast;
    }

    public override bool Check()
    {
        return currCasts >= numCasts;
    }
}
