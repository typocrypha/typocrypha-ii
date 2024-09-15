using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerCooldownEffect : RootWordEffect
{
    public int amount;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        var results = new CastResults(caster, target)
        {
            Mod = mod.specialModifier,
            DisplayDamage = false,
        };
        Damage.StandardHitCheck(results, this, caster, target);
        if (!target.IsPlayer)
        {
            results.Effectiveness = Reaction.Block;
            return results;
        }
        if (mod.specialModifier == Damage.SpecialModifier.CritBlock)
        {
            results.Miss = true;
            return results;
        }
        if (results.Miss)
            return results;
        results.Effectiveness = Damage.GetReaction(this, caster, target, out float mult);
        if (Damage.ApplyReflect(results, this, caster, target, spellData))
            return results;
        SpellCooldownManager.instance.LowerAllCooldowns(amount);
        SpellCooldownManager.instance.SortCooldowns();
        return results;
    }
}
