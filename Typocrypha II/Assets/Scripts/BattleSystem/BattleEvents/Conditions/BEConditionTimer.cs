using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks whether a certain amount of time has passed.
/// </summary>
public class BEConditionTimer : BattleEventCondition
{
    public float time = 10f;
    bool done;

    void Awake()
    {
        StartCoroutine(Timer());
    }

    public override bool Check()
    {
        return done;
    }

    IEnumerator Timer()
    {
        done = false;
        yield return new WaitForSeconds(time);
        done = true;
    }
}
