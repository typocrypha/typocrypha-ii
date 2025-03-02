using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks whether a certain amount of time has passed.
/// </summary>
public abstract class BEConditionTimerBase : BattleEventCondition
{
    protected abstract float Time { get; }
    bool done;

    protected override void Start()
    {
        base.Start();
        StartTimer();
    }

    private void StartTimer()
    {
        StartCoroutine(Timer(Time));
    }

    protected override bool CheckInternal()
    {
        return done;
    }

    public override void ResetValues()
    {
        StopAllCoroutines();
        done = false;
        StartTimer();
    }

    IEnumerator Timer(float time)
    {
        done = false;
        float currTime = 0f;
        var waitWhile = new WaitWhile(WaitWhile);
        var fixedUpdate = new WaitForFixedUpdate();
        while (currTime < time)
        {
            yield return waitWhile;
            yield return fixedUpdate;
            currTime += UnityEngine.Time.fixedDeltaTime;
        }
        done = true;
    }

    protected virtual bool WaitWhile()
    {
        return battleEvent.PH.Paused;
    }
}
