using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FomulaCritCountered : CustomFormula
{
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.SpecialModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        var results = Damage.Standard(effect, caster, target, target.Countered ? Damage.SpecialModifier.Critical : mod, spellData);
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
