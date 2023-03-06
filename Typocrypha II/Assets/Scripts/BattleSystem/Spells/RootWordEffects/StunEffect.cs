using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : RootWordEffect
{
    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target);
        Damage.StandardHitCheck(results, this, caster, target);
        Damage.StandardElements(results, this, caster, target);
        results.StaggerDamage = target.Stagger;
        results.DisplayDamage = false;
        Damage.ApplyStandard(results, this, caster, target, spellData);
        return results;
    }
}
