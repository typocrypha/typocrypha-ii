using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDefenseless : CasterAbility
{
    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if (!target.Stunned || castResults.Effectiveness == Reaction.Weak)
        {
            return;
        }
        // Undo previous reaction
        castResults.Damage *= (1 / Damage.GetReactionDmgMod(effect, caster, target, castResults.Effectiveness, castResults.EffectivenessMagnitude));
        castResults.Effectiveness = Reaction.Weak;
        castResults.EffectivenessMagnitude = 1;
        castResults.Damage *= Damage.GetReactionDmgMod(effect, caster, target, castResults.Effectiveness, castResults.EffectivenessMagnitude);
        if(castResults.StaggerDamage <= 0)
        {
            castResults.StaggerDamage = 1;
        }
    }

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {
        return;
    }
}
