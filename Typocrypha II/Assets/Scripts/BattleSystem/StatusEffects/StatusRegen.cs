using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRegen : StatusRemoveAfterHitOrCast
{
    public float tickTime;
    public int healthPerTick;
    private float time = 0;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if(time >= tickTime)
        {
            affected.Health += healthPerTick;
            time = 0;
        }
    }
}
