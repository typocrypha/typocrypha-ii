using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Damage
{
    public const float stunBonusDamageMod = 1.25f;
    public const float critDamageMod = 2f;
    public const float baseCritChance = 0.1f;

    public enum FormulaType
    {
        Standard,
        Custom,
    }
    public delegate CastResults Formula(DamageEffect effect, Caster caster, Caster target);

    public static Dictionary<FormulaType, Formula> PresetFormulae { get; } = new Dictionary<FormulaType, Formula>
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
        var results = new CastResults(caster, target);
        StandardHitCheck(results, effect, caster, target);
        StandardAtkDef(results, effect, caster, target);
        if (StandardCritCheck(results, effect, caster, target))
            StandardCritMod(results, effect, caster, target);
        StandardElements(results, effect, caster, target);
        StandardPower(results, effect, caster, target);
        StandardStunBonus(results, effect, caster, target);
        return results;
    }

    /// <summary>
    /// Sets the Hit property of the given results based on a standard accuracy check
    /// </summary>
    public static void StandardHitCheck(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        // If the move always hits, bypass accuracy checks
        if(effect.tags.Contains("AlwaysHit"))
        {
            results.Miss = false;
            return;
        }
        // If the move always misses, bypass accuracy checks
        else if (effect.tags.Contains("AlwaysMiss"))
        {
            results.Miss = true;
            return;
        }
        float hitChance = 1 * CompareStats(caster.Stats.Acc, target.Stats.Evade);
        results.Miss = (Random.Range(0, 1) > hitChance);
    }
    /// <summary>
    /// Sets the Crit property and modifies damage and stagger based on a standard crit check
    /// </summary>
    public static bool StandardCritCheck(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        // If the move always crits, bypass accuracy checks
        if (effect.tags.Contains("AlwaysCrit"))
        {
            return true;
        }
        // If the move always misses, bypass accuracy checks
        else if (effect.tags.Contains("NeverCrit"))
        {
            return false;
        }
        float critChance = baseCritChance;
        if(Random.Range(0,1f) <= critChance)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Applies the standard modification to the results
    /// Sets crit to true, sets stagger damage to 1, and multiplies the Damage by Damage.critDamageMod;
    /// </summary>
    public static void StandardCritMod(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        results.Crit = true;
        results.StaggerDamage = 1;
        results.Damage *= critDamageMod;
    }

    /// <summary>
    /// Apply standard stat modifiers to a spell.
    /// </summary>
    public static void StandardAtkDef(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        results.Damage *= CompareStats(caster.Stats.Atk, target.Stats.Def);
    }

    /// <summary>
    /// Compare two stats by a standard comparison. Returns a non-negative multiplier
    /// Returns a constant base raised to the power of the difference between the two stats.
    /// </summary>
    public static float CompareStats(int stat1, int stat2)
    {
        const float expBase = 1.25f;
        float difference = stat1 - stat2;
        return Mathf.Pow(expBase, difference);
    }
    /// <summary>
    /// Modifies damage by power by simple multiplication
    /// </summary>
    public static void StandardPower(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        results.Damage *= effect.power;
    }
    /// <summary>
    /// Applies a standard stun bonus to damage if the target is already stunned and the attack would deal stagger damager
    /// Should be applied at the end of calculation
    /// </summary>
    public static void StandardStunBonus(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        if (target.Stunned && results.StaggerDamage > 0)
            results.Damage *= stunBonusDamageMod;
    }

    #region Standard Elements and Reactions
    /// <summary>
    /// Applies the standard element modifiers to a running CastResults passed in as results
    /// Calculates Effectiveness modifiers by GetReaction
    /// Calculates Damage multiplier by GetReactionDmgMod
    /// Adds Stagger Damage if the reaction is weak
    /// </summary>
    public static void StandardElements(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        results.Effectiveness = GetReaction(effect, caster, target, out float effectMagnitude);
        results.Damage *= GetReactionDmgMod(effect, caster, target, results.Effectiveness, effectMagnitude);
        if (results.Effectiveness == Reaction.Weak)
            results.StaggerDamage = 1;
    }
    /// <summary>
    /// Get the reaction of the given effect on the given target.
    /// The strongest reaction is taken, with its multiplier being the number of that reaction the target has
    /// Strength order = Repel -> Drain -> Dodge -> Block -> BASIC.
    /// If BASIC is reached, the reaction is determined by comparing the number of resists with the number of weaks
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
                var r = target.GetReactions(tag);
                if (r != null) reactions.AddSet(r);
            }
        }
        // If any of tags are repelled, repel (unless the spell cannot be repelled or has already been repelled
        if (reactions.Contains(Reaction.Repel) && !effect.tags.Contains("IgnoreReflect") && !effect.tags.Contains("Reflected"))
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
        if (results.Miss)
            return;
        if (ApplyReflect(results, effect, caster, target))
            return;
        ApplyDamage(results, effect, caster, target);
        ApplyStaggerDamage(results, effect, caster, target);
        ApplyStatusEffects(results, effect, caster, target);
    }

    public static bool ApplyReflect(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        if (results.Effectiveness == Reaction.Repel)
        {
            effect.tags.Add("Reflected");
            effect.Cast(caster, caster);
            return true;
        }
        return false;
    }

    public static void ApplyDamage(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        target.Health -= Mathf.FloorToInt(results.Damage);
    }
    /// <summary>
    /// Apply stagger damage and log a message if stunned
    /// </summary>
    public static void ApplyStaggerDamage(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        if (target.Stunned)
            return;
        target.Stagger -= Mathf.FloorToInt(results.StaggerDamage);
        if (target.Stunned)
            SpellFxManager.instance.LogMessage(target.DisplayName + " is stunned!");
    }

    #region Apply Special Tags
    /// <summary>
    /// Apply "Status Conditions" based on the prescence of certain tags
    /// Specifically, this function hadles anything that could affect the player's keyboard if the target is the player
    /// </summary>
    public static void ApplyStatusEffects(CastResults results, DamageEffect effect, Caster caster, Caster target)
    {
        //Inflict keyboard effects if the target is the player
        if(target.CasterClass == Caster.Class.Player)
        {
            if (effect.tags.Contains("Fire"))
            {
                Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectBurning");
            }
            if(effect.tags.Contains("Lightning"))
            {
                Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectShocked");
            }
            if(effect.tags.Contains("Ice"))
            {
                Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectFrozen");
            }
        }

    }

    #endregion

    #endregion


}
