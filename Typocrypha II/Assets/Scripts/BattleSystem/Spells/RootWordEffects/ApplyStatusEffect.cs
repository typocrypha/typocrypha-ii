using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffect : RootWordEffect
{
    public GameObject statusEffectPrefab;
    public bool canCrit = false;

    public override bool CanCrit => canCrit;
    protected StatusEffect Effect => statusEffectPrefab.GetComponent<StatusEffect>();

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        var results = new CastResults(caster, target)
        {
            Mod = mod,
        };
        Damage.StandardHitCheck(results, this, caster, target);
        if(mod == Damage.SpecialModifier.CritBlock)
        {
            results.Miss = true;
            return results;
        }
        if (results.Miss)
            return results;
        results.Effectiveness = Damage.GetReaction(this, caster, target, out float mult);
        results.DisplayDamage = false;
        if (Damage.ApplyReflect(results, this, caster, target, spellData))
            return results;
        var baseEffect = Effect;
        if(target.GetStatusEffect(baseEffect.casterTag) != null)
        {
            results.Miss = true;
            var failMessage = baseEffect.FailMessage(caster, target);
            if(!string.IsNullOrEmpty(failMessage))
            {
                LogMessage(failMessage);
            }
            return results;
        }
        var effect = Instantiate(statusEffectPrefab, target.transform).GetComponent<StatusEffect>();
        target.AddTagWithStatusEffect(effect, effect.casterTag);
        effect.Apply(this, caster, target, results);
        return results;
    }
}
