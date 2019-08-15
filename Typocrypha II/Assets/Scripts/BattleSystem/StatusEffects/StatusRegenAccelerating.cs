using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRegenAccelerating : StatusRemoveAfterHitOrCast
{
    public float tickTime;
    public float minTick;
    public int healthPerTick;
    public float decreasePerTick;
    private float time;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if (time >= tickTime)
        {
            affected.Health += healthPerTick;
            SpellFxManager.instance.PlayDamageNumber(-healthPerTick, affected.transform.position);
            tickTime -= decreasePerTick;
            if (tickTime < minTick)
                tickTime = minTick;
            time = 0;
        }
    }
}
