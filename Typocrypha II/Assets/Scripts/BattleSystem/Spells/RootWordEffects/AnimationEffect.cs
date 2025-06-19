using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        return InitializeCastResults(caster, target, mod);
    }
}
