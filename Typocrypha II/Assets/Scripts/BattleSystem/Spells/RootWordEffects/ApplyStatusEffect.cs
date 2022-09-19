using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffect : RootWordEffect
{
    public GameObject statusEffectPrefab;

    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, bool crit, Spell spell, RootCastResults prevResults = null)
    {
        var results = new CastResults(caster, target);
        Damage.StandardHitCheck(results, this, caster, target);
        if (results.Miss)
            return results;
        results.Effectiveness = Damage.GetReaction(this, caster, target, out float mult);
        results.DisplayDamage = false;
        if (Damage.ApplyReflect(results, this, caster, target, spell))
            return results;
        var effect = Instantiate(statusEffectPrefab, target.transform).GetComponent<StatusEffect>();
        effect.Apply(this, caster, target, results);
        target.AddTagWithStatusEffect(effect, effect.casterTag);
        return results;
    }
}
