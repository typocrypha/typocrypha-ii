using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks whether a certain amount of time has passed.
/// </summary>
public class BEConditionTimer : BEConditionTimerBase
{
    public float time;

    protected override float Time => time;
}
