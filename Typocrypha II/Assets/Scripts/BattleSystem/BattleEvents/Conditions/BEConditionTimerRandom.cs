using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionTimerRandom : BEConditionTimerBase
{
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    protected override float Time => Mathf.Lerp(minTime, maxTime, (float)RandomUtils.RandomU.instance.RandomDouble());
}
