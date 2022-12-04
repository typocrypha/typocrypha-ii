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
        if (data.IsCrit)
        {
            healthPerTick *= (int)Damage.critDamageMod;
        }
    }

    void FixedUpdate()
    {
        if (ATB3.ATBManager.instance.InSolo)
            return;
        time += Time.fixedDeltaTime;
        if(time >= tickTime)
        {
            if (affected.BStatus == Caster.BattleStatus.Normal && affected.Health == affected.Stats.MaxHP)
                return;
            if (affected.BStatus == Caster.BattleStatus.SpiritMode && affected.SP == affected.Stats.MaxSP)
                return;
            affected.Heal(healthPerTick);
            SpellFxManager.instance.PlayDamageNumber(-healthPerTick, affected.transform.position);
            time = 0;
        }
    }
}
