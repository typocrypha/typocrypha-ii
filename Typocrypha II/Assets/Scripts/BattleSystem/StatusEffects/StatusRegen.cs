using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRegen : StatusRemoveAfterHitOrCast
{
    public float tickTime;
    public int healthPerTick;
    private float time = 0;

    public override void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data)
    {
        if (data.Crit)
        {
            healthPerTick *= (int)Damage.critDamageMod;
        }
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if(time >= tickTime)
        {
            affected.Heal(healthPerTick);
            SpellFxManager.instance.PlayDamageNumber(-healthPerTick, affected.transform.position);
            time = 0;
        }
    }
}
