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

    public override void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data)
    {
        if (data.IsCrit)
        {
            healthPerTick *= (int)Damage.critDamageMod;
        }
    }

    void FixedUpdate()
    {
        if (ATB3.ATBManager.instance.ProcessingActions)
            return;
        time += Time.fixedDeltaTime;
        if (time >= tickTime)
        {
            if (affected.BStatus == Caster.BattleStatus.Normal && affected.Health == affected.Stats.MaxHP)
                return;
            if (affected.BStatus == Caster.BattleStatus.SpiritMode && affected.SP == affected.Stats.MaxSP)
                return;
            affected.Heal(healthPerTick);
            SpellFxManager.instance.PlayDamageNumber(-healthPerTick, affected.transform.position);
            tickTime -= decreasePerTick;
            if (tickTime < minTick)
                tickTime = minTick;
            time = 0;
        }
    }
}
