using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FormulaDrainDamage : CustomFormula
{
    public CasterTag filterTag;
    public float drainMod = 0.5f;
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.DamageModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        if (prevResults == null || prevResults.Count <= 0)
            return null;
        var results = new CastResults(caster, target);
        results.Damage = 0;
        foreach (var r in prevResults.LastEffect)
        {
            if (!r.WillDealDamage || (filterTag != null && !r.target.HasTag(filterTag)))
                continue;
            results.Damage -= r.Damage;
        }
        if (results.Damage >= 0)
            return null;
        results.Damage *= drainMod;
        if (mod.specialModifier == Damage.SpecialModifier.Critical)
        {
            results.Mod = Damage.SpecialModifier.Critical;
            results.Damage *= Damage.critDamageMod;
        }
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
