﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Check if the player has been hit by X damaging spells
/// </summary>
public class BEConditionPlayerHit : BattleEventCondition
{
    [SerializeField] private int numCasts;
    private int currCasts = 0;

    protected override void AddEventHandlers()
    {
        Battlefield.instance.Player.OnBeforeHitResolved += CheckCast;
    }

    protected override void RemoveEventHandlers()
    {
        Battlefield.instance.Player.OnBeforeHitResolved -= CheckCast;
    }

    public void CheckCast(RootWordEffect effect,Caster caster,Caster target, CastResults data)
    {
        if (!data.WillDealDamage)
            return;
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