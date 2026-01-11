using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FomulaFalseSwipe : CustomFormula
{
    [SerializeField] private int threshold = 1;
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.DamageModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        var results = Damage.Standard(effect, caster, target, mod, spellData);
        if(results.Damage > target.Health - threshold)
        {
            results.Damage = target.Health - threshold;
        }
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
