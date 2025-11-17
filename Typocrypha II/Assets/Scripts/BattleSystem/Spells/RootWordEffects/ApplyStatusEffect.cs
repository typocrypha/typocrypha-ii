using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffect : RootWordEffect
{
    public GameObject statusEffectPrefab;
    public bool canCrit = false;

    public override bool CanCrit => canCrit;
    protected StatusEffect Effect => statusEffectPrefab.GetComponent<StatusEffect>();

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        var results = InitializeCastResults(caster, target, mod);
        if (mod.specialModifier == Damage.SpecialModifier.CritBlock)
        {
            results.Miss = false;
            results.Effectiveness = Reaction.Block;
            return results;
        }
        Damage.StandardHitCheck(results, this, caster, target);
        if (results.Miss)
            return results;
        results.Effectiveness = Damage.GetReaction(this, caster, target, out float mult);
        if (results.Effectiveness == Reaction.Block)
            return results;
        if (Damage.ApplyReflect(results, this, caster, target, spellData))
            return results;
        // Actually apply effect
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
