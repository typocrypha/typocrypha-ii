using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionMariOrbSpawnTimer : BEConditionTimerRandom
{
    [SerializeField] private float minTimeDuringFieldFull;
    protected override bool WaitWhile(float currTime)
    {
        return base.WaitWhile(currTime) || (currTime <= minTimeDuringFieldFull && Battlefield.instance.NumValidReinforcementPositions <= 0);
    }
}
