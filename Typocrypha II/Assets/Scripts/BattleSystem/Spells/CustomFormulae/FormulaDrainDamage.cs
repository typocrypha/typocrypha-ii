using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FormulaDrainDamage : CustomFormula
{
    public float drainMod = 0.5f;
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.SpecialModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        var results = new CastResults(caster, target);
        if (prevResults == null || prevResults.Count <= 0)
            return results;
        results.Damage = -(prevResults.LastEffect.Sum((r) => r.WillDealDamage ? r.Damage : 0));
        if (results.Damage > 0)
            results.Damage = 0;
        results.Damage *= drainMod;
        if (mod == Damage.SpecialModifier.Critical)
        {
            results.Mod = Damage.SpecialModifier.Critical;
            results.Damage *= Damage.critDamageMod;
        }
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
