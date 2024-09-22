using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        TransitionManager.instance.TransitionToNextScene();
        return new CastResults(caster, target);
    }
}
