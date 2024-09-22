using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectMerciless : BadgeEffectOnBeforeSpellEffectCast
{
    [SerializeField] private float multiplier = 0.25f;

    protected override void ApplyEffect(RootWordEffect effect, Caster caster, Caster target, Damage.DamageModifier mod)
    {
        int count = 0;
        if (target.Stunned)
        {
            count++;
        }
        if (target.Countered)
        {
            count++;
        }
        if (target.Running)
        {
            count++;
        }
        if (count <= 0)
            return;
        mod.damageMultiplier += (multiplier * count);
    }
}
