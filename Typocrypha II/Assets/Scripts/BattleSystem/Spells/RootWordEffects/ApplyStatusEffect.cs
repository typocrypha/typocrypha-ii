using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffect : RootWordEffect
{
    public GameObject statusEffectPrefab;
    public override CastResults Cast(Caster caster, Caster target)
    {
        var results = new CastResults(caster, target);
        Damage.StandardHitCheck(results, this, caster, target);
        if (results.Miss)
            return results;
        Damage.StandardCritCheck(results, this, caster, target);
        results.Effectiveness = Damage.GetReaction(this, caster, target, out float mult);
        results.DisplayDamage = false;
        if (Damage.ApplyReflect(results, this, caster, target))
            return results;
        var effect = Instantiate(statusEffectPrefab, target.transform).GetComponent<StatusEffect>();
        effect.Apply(this, caster, target, results);
        target.AddTagWithStatusEffect(effect, effect.casterTag);
        return results;
    }
}
