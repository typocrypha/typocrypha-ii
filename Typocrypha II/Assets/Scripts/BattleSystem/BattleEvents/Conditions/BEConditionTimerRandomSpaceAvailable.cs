using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionTimerRandomSpaceAvailable : BEConditionTimerRandom
{
    protected override bool WaitWhile()
    {
        return base.WaitWhile() || Battlefield.instance.NumValidReinforcementPositions <= 0;
    }
}
