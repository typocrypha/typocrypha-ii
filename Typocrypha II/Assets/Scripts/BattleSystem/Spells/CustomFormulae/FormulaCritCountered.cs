using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FormulaCritCountered : CustomFormula
{
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.DamageModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        var dmgMod = target.Countered ? new Damage.DamageModifier(mod) { specialModifier = Damage.SpecialModifier.Critical } : mod;
        var results = Damage.Standard(effect, caster, target, dmgMod, spellData);
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
