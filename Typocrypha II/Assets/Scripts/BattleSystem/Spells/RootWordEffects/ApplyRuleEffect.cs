using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyRuleEffect : RootWordEffect
{
    public enum RuleEnum
    {
        RemoveRule,
        BurningJustice,
    }
    public override bool CanCrit => false;

    public RuleEnum ruleType;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        var results = new CastResults(caster, target, 0)
        {
            DisplayDamage = false,
            Miss = false,
        };
        if(ruleType == RuleEnum.RemoveRule)
        {
            Rule.ActiveRule = null;
            return results;
        }
        Rule rule;
        switch (ruleType)
        {
            case RuleEnum.BurningJustice:
                rule = new RuleBurningJustice();
                break;
            default:
                rule = null;
                break;
        }
        if (rule != null)
        {
            Rule.ActiveRule = rule;
        }
        return results;
    }
}
