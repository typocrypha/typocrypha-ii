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

        target.Health -= Mathf.FloorToInt(effect.power * damageMod);
        return new PopupData
        {
            damage = effect.power,
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
            if(target.Tags.GetReactions(tag) != null)
                reactions.AddSet(target.Tags.GetReactions(tag));
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
            // Sum up weaknesses and resistances, return sum
            multiplier = reactions.Freq(Reaction.Resist) - reactions.Freq(Reaction.Weak);
            if (multiplier > 0)
                return Reaction.Resist;
            if(multiplier < 0)
            {
                multiplier *= -1;
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
