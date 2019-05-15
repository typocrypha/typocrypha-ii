using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks whether a certain amount of time has passed.
/// </summary>
public class BEConditionTimer : BattleEventCondition
{
    public float time;
    bool done;

    void Start()
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
        float currTime = 0f;
        while (currTime < time)
        {
            yield return new WaitWhile(() => battleEvent.PH.Pause);
            yield return new WaitForFixedUpdate();
            currTime += Time.fixedDeltaTime;
        }
        done = true;
    }
}
