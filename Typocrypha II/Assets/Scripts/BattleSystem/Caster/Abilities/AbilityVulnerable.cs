using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityVulnerable : AbilityIncomingDamageMultiplier
{
    protected override float Multiplier => 2;
    protected override bool ShouldApplyAbility(RootWordEffect effect, Caster caster, Caster self, CastResults castResults)
    {
        return self.Stunned;
    }
}
