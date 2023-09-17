using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Damage
{
    public const float stunBonusDamageMod = 1.25f;
    public const float critDamageMod = 2f;
    public const float baseCritChance = 0.075f;

    public enum FormulaType
    {
        StandardDmg,
        Custom,
        StandardHeal,
    }

    public delegate CastResults Formula(DamageEffect effect, Caster caster, Caster target, SpecialModifier mod, RootCastData spellData);

    public enum SpecialModifier
    {
        None,
        Critical,
        CritBlock
    }

    public static Dictionary<FormulaType, Formula> PresetFormulae { get; } = new Dictionary<FormulaType, Formula>
    {
        { FormulaType.StandardDmg, StandardApplied },
        { FormulaType.StandardHeal, StandardHealApplied },
    };

    private static CastResults StandardApplied(DamageEffect effect, Caster caster, Caster target, SpecialModifier mod, RootCastData spellData)
    {
        var results = Standard(effect, caster, target, mod, spellData);
        ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }

    private static CastResults StandardHealApplied(DamageEffect effect, Caster caster, Caster target, SpecialModifier mod, RootCastData spellData)
    {
        var results = StandardHeal(effect, caster, target, mod, spellData);
        ApplyStandard(results, effect, caster, target, spellData);
        return results;
    }

    #region Calculation

    public static CastResults Standard(DamageEffect effect, Caster caster, Caster target, SpecialModifier mod, RootCastData spellData)
    {
        var results = new CastResults(caster, target, 1);
        StandardHitCheck(results, effect, caster, target);
        StandardAtkDef(results, effect, caster, target);
        StandardElements(results, effect, caster, target);
        StandardSpecialMod(results, effect, caster, target, mod);
        StandardPower(results, effect, caster, target);
        ComputeStandardComboValue(results, spellData.Spell);
        ApplyStandardComboMod(results);
        StandardStunBonus(results, effect, caster, target);
        return results;
    }

    public static CastResults StandardHeal(DamageEffect effect, Caster caster, Caster target, SpecialModifier mod, RootCastData spellData)
    {
        var results = new CastResults(caster, target, 1)
        {
            Miss = effect.tags.Contains("AlwaysMiss"),
        };
        StandardElements(results, effect, caster, target);
        StandardSpecialMod(results, effect, caster, target, mod);
        results.StaggerDamage = 0;
        StandardPower(results, effect, caster, target);
        ComputeStandardComboValue(results, spellData.Spell);
        ApplyStandardComboMod(results);
        results.Damage *= -1;
        return results;
    }

    /// <summary>
    /// Sets the Hit property of the given results based on a standard accuracy check
    /// </summary>
    public static void StandardHitCheck(CastResults results, RootWordEffect effect, Caster caster, Caster target)
    {
        // If the move always hits or the target is stunned, bypass accuracy checks
        if(effect.tags.Contains("AlwaysHit") || target.Stunned)
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
        float hitChance = 1 * CompareStatsWeak(caster.Stats.Acc, target.Stats.Evade);
        results.Miss = (Random.Range(0f, 1f) > hitChance);
    }
    /// <summary>
    /// Sets the Crit property and modifies damage and stagger based on a standard crit check
    /// </summary>
    public static bool StandardCritCheck(RootWordEffect effect, Caster caster, Caster target)
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
        float critChance = baseCritChance * CompareStats(caster.Stats.Luck, target.Stats.Luck);
        return Random.Range(0, 1f) <= critChance;
    }

    /// <summary>
    /// Sets the Crit property and modifies damage and stagger based on a standard crit check
    /// </summary>
    public static bool StandardCritCheckBuff(CastResults results, RootWordEffect effect, Caster caster, Caster target)
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
        float critChance = baseCritChance + (0.025f * (caster.Stats.Luck + target.Stats.Luck));
        return Random.Range(0, 1f) <= critChance;
    }

    /// <summary>
    /// Applies the standard modification to the results
    /// Sets crit to true, sets stagger damage to 1, and multiplies the Damage by Damage.critDamageMod;
    /// </summary>
    public static void StandardSpecialMod(CastResults results, RootWordEffect effect, Caster caster, Caster target, SpecialModifier mod)
    {
        results.Mod = mod;
        if (mod == SpecialModifier.Critical)
        {
            results.Damage *= critDamageMod;
            results.StaggerDamage = 1;
        }
        else if(mod == SpecialModifier.CritBlock)
        {
            results.Effectiveness = Reaction.Block;
            results.Damage *= GetReactionDmgMod(effect, caster, target, Reaction.Block, 1);
        }
    }

    public static void ApplyStandardComboMod(CastResults results)
    {
        results.Damage *= StandardComboMod(results.Combo);
    }

    public static float StandardComboMod(float comboValue)
    {
        return 1 + (comboValue * 0.15f);
    }

    public static void ComputeStandardComboValue(CastResults results, Spell spell)
    {
        results.Combo = spell.Count - 1;
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
    /// Compare two stats by a standard comparison. Returns a non-negative multiplier
    /// Returns a constant base raised to the power of the difference between the two stats.
    /// </summary>
    public static float CompareStatsWeak(int stat1, int stat2)
    {
        const float expBase = 1.075f;
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
    public static void StandardStunBonus(CastResults results, RootWordEffect effect, Caster caster, Caster target)
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
    public static void StandardElements(CastResults results, RootWordEffect effect, Caster caster, Caster target)
    {
        results.Effectiveness = GetReaction(effect, caster, target, out float effectMagnitude);
        results.EffectivenessMagnitude = effectMagnitude;
        ApplyReaction(results, effect, caster, target);
    }
    public static void ApplyReaction(CastResults results, RootWordEffect effect, Caster caster, Caster target)
    {
        results.Damage *= GetReactionDmgMod(effect, caster, target, results.Effectiveness, results.EffectivenessMagnitude);
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
    public static Reaction GetReaction(RootWordEffect effect, Caster caster, Caster target, out float multiplier)
    {
        multiplier = 0;
        var reactions = new CasterTagDictionary.ReactionMultiSet();
        // Compute cumulative reactions
        foreach (var tag in effect.tags)
        {
            if (tag == null)
            {
                continue;
            }
            var tagReactions = target.GetReactions(tag);
            if (tagReactions != null) reactions.AddSet(tagReactions);
            if (target.ExtraReactions != null)
            {
                foreach (Caster.GetReactionsFn deleg in target.ExtraReactions.GetInvocationList())
                {
                    var extraReactions = deleg.Invoke(tag);
                    if (extraReactions != null)
                        reactions.AddSet(extraReactions);
                }
            }
        }
        // If any of tags are repelled, repel (unless the spell cannot be repelled or has already been repelled)
        if (reactions.Contains(Reaction.Repel) && !effect.tags.Contains("IgnoreReflect") && !effect.tags.Contains("Reflected"))
        {
            multiplier = reactions.Freq(Reaction.Repel);
            return Reaction.Repel;
        }
        // Else if any drain, drain
        if (reactions.Contains(Reaction.Drain))
        {
            multiplier = reactions.Freq(Reaction.Drain);
            return Reaction.Block; // Temp, while drain is being reimplemented
        }
        #region  Dodge (currently deprecated)
        //// If any dodge, dodge
        //else if (reactions.Contains(Reaction.Dodge))
        //{
        //    multiplier = reactions.Freq(Reaction.Dodge);
        //    return Reaction.Dodge;
        //}
        #endregion
        // If the target blocks any of the tags, return block
        if (reactions.Contains(Reaction.Block))
        {
            multiplier = reactions.Freq(Reaction.Block);
            return Reaction.Block;
        }
        // If the spell isn't blocked or repelled, compare number of weaknesses to resistances
        multiplier = reactions.Freq(Reaction.Weak) - reactions.Freq(Reaction.Resist);
        if(multiplier < 0)
        {
            multiplier = Mathf.Abs(multiplier);
            return Reaction.Resist;
        }
        return multiplier > 0 ? Reaction.Weak : Reaction.Neutral;
    }
    /// <summary>
    /// Get the standard damage multiplier for a given reaction.
    /// magnitude is the number of that reaction
    /// </summary>
    public static float GetReactionDmgMod(RootWordEffect effect, Caster caster, Caster target, Reaction r, float mag)
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

    public static void ApplyStandard(CastResults results, RootWordEffect effect, Caster caster, Caster target, RootCastData spellData)
    {
        target.OnBeforeHitResolved?.Invoke(effect, caster, target, spellData, results);
        if (results.Miss)
            return;
        if (ApplyReflect(results, effect, caster, target, spellData))
            return;
        ApplyResearch(results, effect, caster, target);
        ApplyDamage(results, effect, caster, target);
        ApplyStaggerDamage(results, effect, caster, target);
        ApplyKeyboardEffects(results, effect, caster, target);
    }

    public static bool ApplyReflect(CastResults results, RootWordEffect effect, Caster caster, Caster target, RootCastData spellData)
    {
        if (results.Effectiveness == Reaction.Repel)
        {
            effect.tags.Add("Reflected");
            var newResults = effect.Cast(caster, caster, spellData, results.Mod);
            caster.OnAfterHitResolved?.Invoke(effect, caster, caster, spellData, newResults);
            return true;
        }
        return false;
    }

    public static void ApplyDamage(CastResults results, RootWordEffect effect, Caster caster, Caster target)
    {
        target.Damage(Mathf.FloorToInt(results.Damage));
    }
    /// <summary>
    /// Apply stagger damage and log a message if stunned
    /// </summary>
    public static void ApplyStaggerDamage(CastResults results, RootWordEffect effect, Caster caster, Caster target)
    {
        if (target.Stunned || !MoveDoesDamage(results.Effectiveness))
            return;
        target.Stagger -= Mathf.FloorToInt(results.StaggerDamage);
        if (target.Stunned)
        {
            results.Stun = true;
            //SpellFxManager.instance.LogMessage(target.DisplayName + " is stunned!");
        }

    }

    public static void ApplyResearch(CastResults results, RootWordEffect effect, Caster caster, Caster target)
    {
        if(!MoveDoesDamage(results.Effectiveness))
            return;
        if (string.IsNullOrWhiteSpace(target.ResearchKey))
            return;
        if (caster.IsPlayer && target.BStatus == Caster.BattleStatus.SpiritMode)
        {
            var research = PlayerDataManager.instance.researchData;
            research.Add(target.ResearchKey, target.ResearchAmount);
            if (research.ReadyToDecode(target.ResearchKey))
            {
                var word = research.GetData(target.ResearchKey)?.unlockedWord;
                if (word == null)
                    return;
                research.SetDecodeInProgress(target.ResearchKey);
                IEnumerator LogDecoded(bool success)
                {
                    if (success)
                    {
                        research.SetDecoded(target.ResearchKey);
                        if (word != null)
                        {
                            PlayerDataManager.instance.equipment.UnlockWord(word, true);
                            SpellCooldownManager.instance.AddWord(word, true);
                        }
                    }
                    else
                    {
                        research.CancelDecodeInProgress(target.ResearchKey);
                    }
                    return null;
                }
                SpellManager.instance.LogDecodePopup("Decode Chance!", target.ResearchKey, 0, LogDecoded);
            }
        }
    }

    #region Apply Special Tags
    /// <summary>
    /// Apply "Status Conditions" based on the prescence of certain tags
    /// Specifically, this function hadles anything that could affect the player's keyboard if the target is the player
    /// </summary>
    public static void ApplyKeyboardEffects(CastResults results, RootWordEffect effect, Caster caster, Caster target)
    {
        //Inflict keyboard effects if the target is the player
        if(target.IsPlayer)
        {
            if (effect.tags.Contains("Fire"))
            {
                Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectBurning", 3);
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

    public static bool MoveDoesDamage(Reaction r)
    {
        return r == Reaction.Neutral || r == Reaction.Resist || r == Reaction.Weak;
    }

}
