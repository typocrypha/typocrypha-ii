using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks an caster's health.
/// </summary>
public class BEConditionPlayerCast : BattleEventCondition
{
    public int numCasts;
    private int currCasts = 0;

    void Start()
    {
        Battlefield.instance.Player.OnBeforeEffectApplied += CheckCast;
    }

    private void OnDestroy()
    {
        if (currCasts < numCasts)
            Battlefield.instance.Player.OnBeforeEffectApplied -= CheckCast;
    }

    public void CheckCast(RootWordEffect effect,Caster caster,Caster target, CastResults data)
    {
        currCasts++;
        if (currCasts >= numCasts)
            Battlefield.instance.Player.OnBeforeEffectApplied -= CheckCast;
    }

    public override bool Check()
    {
        return currCasts >= numCasts;
    }
}
