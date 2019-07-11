using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Damage
{
    public enum FormulaType
    {
        Standard,
        Custom,
    }
    public delegate CastResults Formula(DamageEffect effect, Caster caster, Caster target);

    public static Dictionary<FormulaType, Formula> PresetFormula { get; } = new Dictionary<FormulaType, Formula>
    {
        { FormulaType.Standard, StandardApplied }
    };

    public static CastResults StandardApplied(DamageEffect effect, Caster caster, Caster target)
    {
        var results = Standard(effect, caster, target);
        ApplyStandard(results, effect, caster, target);
        return results;
    }

    #region Calculation

    public static CastResults Standard(DamageEffect effect, Caster caster, Caster target)
    {
        var results = StandardElements(effect, caster, target);
        results.Damage *= effect.power;
        return results;
    }

    #region Standard Elements and Reactions

    public static CastResults StandardElements(DamageEffect effect, Caster caster, Caster target)
    {
        float effectMagnitude;
        var effective = GetReaction(effect, caster, target, out effectMagnitude);
        var damageMod = GetReactionDmgMod(effect, caster, target, effective, effectMagnitude);
        return new CastResults
        {
            Damage = damageMod,
            Effectiveness = effective,
            StaggerDamage = effective == Reaction.Weak ? 1 : 0
        };
    }
    /// <summary>
    /// Get the reaction of the given effect on the given target.
    /// The strongest reaction is taken, with its multiplier being the number of that reaction the target has
    /// Strength order = Repel -> Drain -> Dodge -> Block -> BASIC.
    /// If BASIC is reached, the reaction is dermened by comparing the number of resists with the number of weaks
    /// to be weak, resist, or neutral
    /// </summary>
    public static Reaction GetReaction(DamageEffect effect, Caster caster, Caster target, out float multiplier)
    {
        multiplier = 0;
        var reactions = new CasterTagDictionary.ReactionMultiSet();
        // Compute cumulative reactions
        foreach (var tag in effect.tags)
        {
            if (tag != null)
            {
                var r = target.Tags.GetReactions(tag);
                if (r != null) reactions.AddSet(r);
            }
        }
        // If any of tags are repelled, repel
        if (reactions.Contains(Reaction.Repel))
        {
            multiplier = reactions.Freq(Reaction.Repel);
            return Reaction.Repel;
        }
        // Else if any drain, drain
        else if (reactions.Contains(Reaction.Drain))
        {
            multiplier = reactions.Freq(Reaction.Drain);
            return Reaction.Drain;
        }
        // If any dodge, dodge
        else if (reactions.Contains(Reaction.Dodge))
        {
            multiplier = reactions.Freq(Reaction.Dodge);
            return Reaction.Dodge;
        }
        // If any block, block
        else if (reactions.Contains(Reaction.Block))
        {
            multiplier = reactions.Freq(Reaction.Block);
            return Reaction.Block;
        }
        else
        {
            // Sum up weaknesses and resistances
            int sum = reactions.Freq(Reaction.Resist) - reactions.Freq(Reaction.Weak);
            if (sum > 0)
            {
                multiplier = sum;
                return Reaction.Resist;
            }
            if (sum < 0)
            {
                // Take the positive value of the sum
                multiplier = -sum;
                return Reaction.Weak;
            }
            return Reaction.Neutral;
        }
    }
    /// <summary>
    /// Get the standard damage multiplier for a given reaction.
    /// magnitude is the number of that reaction
    /// </summary>
    public static float GetReactionDmgMod(DamageEffect effect, Caster caster, Caster target, Reaction r, float mag)
    {
        if (r == Reaction.Repel)
            return mag;
        if (r == Reaction.Weak)
            return (mag + 1);
        if (r == Reaction.Resist)
            return 1 / (mag + 1);
        if (r == Reaction.Neutral)
            return 1;
        if (r == Reaction.Drain)
            return -1 * mag;
        return 0;
    }
    #endregion

    #endregion

    #region Application

    public static void ApplyStandard(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        if (ApplyReflect(results, effect, caster, target))
            return;
        ApplyDamage(results, effect, caster, target);
        ApplyStaggerDamage(results, effect, caster, target);
    }

    public static bool ApplyReflect(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        if (results.Effectiveness == Reaction.Repel && !effect.tags.Contains("IgnoreReflect") && !effect.tags.Contains("Reflected"))
        {
            effect.tags.Add(SpellTag.GetByName("Reflected"));
            effect.Cast(caster, caster);
            return true;
        }
        return false;
    }

    public static void ApplyDamage(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        target.Health -= Mathf.FloorToInt(results.Damage);
    }

    public static void ApplyStaggerDamage(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        if (target.Stunned)
            return;
        target.Stagger -= Mathf.FloorToInt(results.StaggerDamage);
        if (target.Stunned)
            SpellFxManager.instance.LogMessage(target.DisplayName + " is stunned!");
    }

    #endregion


}
