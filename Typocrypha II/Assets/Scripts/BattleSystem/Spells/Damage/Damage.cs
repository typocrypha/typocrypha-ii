﻿using System.Collections;
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
    public delegate PopupData Formula(DamageEffect effect, Caster caster, Caster target);

    public static Dictionary<FormulaType, Formula> StandardFormula { get; } = new Dictionary<FormulaType, Formula>
    {
        { FormulaType.Standard, Standard }
    };   
    private static PopupData Standard(DamageEffect effect, Caster caster, Caster target)
    {
        float effectMagnitude;
        var effective = GetReaction(effect, caster, target, out effectMagnitude);
        if(effective == Reaction.Repel)
        {
            if (target != caster)
            {
                effect.Cast(caster, caster);
                return new PopupData
                {
                    damage = 0,
                    effectiveness = effective,
                };
            }
        }
        var damageMod = GetReactionDmgMod(effect, caster, target, effective, effectMagnitude);
        int damage = Mathf.FloorToInt(effect.power * damageMod);
        target.Health -= damage;
        return new PopupData
        {
            damage = damage,
            effectiveness = effective,
        };
    }
    private static Reaction GetReaction(DamageEffect effect, Caster caster, Caster target, out float multiplier)
    {
        multiplier = 0;
        var reactions = new CasterTagDictionary.ReactionMultiSet();
        // Compute cumulative tags
        foreach(var tag in effect.tags)
        {
            var r = target.Tags.GetReactions(tag);
            if (r != null)
                reactions.AddSet(r);
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
            if(sum < 0)
            {
                multiplier = -sum;
                return Reaction.Weak;
            }
            return Reaction.Neutral;
        }
    }
    private static float GetReactionDmgMod(DamageEffect effect, Caster caster, Caster target, Reaction r, float mag)
    {
        if (r == Reaction.Repel)
            return mag;        
        if(r == Reaction.Weak)
            return (mag + 1);
        if (r == Reaction.Resist)
            return 1 / (mag + 1);
        if (r == Reaction.Neutral)
            return 1;
        if (r == Reaction.Drain)
            return -1 * mag;
        return 0;
    }
    
}
