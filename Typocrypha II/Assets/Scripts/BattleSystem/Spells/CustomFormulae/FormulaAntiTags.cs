using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormulaAntiTags : CustomFormula
{
    [SerializeField] private CasterTag[] tags;
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, Damage.SpecialModifier mod, RootCastData spellData, RootCastResults prevResults = null)
    {
        // Calculate standard results using the standard formula
        bool hasTag = false;
        foreach(var tag in tags)
        {
            if (target.HasTag(tag))
            {
                hasTag = true;
                break;
            }
        }
        CastResults results;
        if (hasTag)
        {
            results = new CastResults(caster, target, 1);
            Damage.StandardHitCheck(results, effect, caster, target);
            Damage.StandardAtkDef(results, effect, caster, target);
            results.Effectiveness = Reaction.Weak;
            results.EffectivenessMagnitude = 1;
            var normalRection = Damage.GetReaction(effect, caster, target, out float normalMagnitude);
            if (normalRection == Reaction.Weak)
            {
                results.EffectivenessMagnitude += normalMagnitude;
            }
            Damage.ApplyReaction(results, effect, caster, target);
            Damage.StandardSpecialMod(results, effect, caster, target, mod);
            Damage.StandardPower(results, effect, caster, target);
            Damage.ComputeStandardComboValue(results, spellData.Spell);
            Damage.ApplyStandardComboMod(results);
            Damage.StandardStunBonus(results, effect, caster, target);
        }
        else
        {
            results = Damage.Standard(effect, caster, target, mod, spellData);
        }
        // Apply the results of the cast in the standard manner
        Damage.ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }
}
