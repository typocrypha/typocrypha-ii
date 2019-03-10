using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierWord", menuName = "Spell Word/Modifier")]
public class ModifierWord : SpellWord
{
    public enum Direction
    {
        Left,
        Right,
        Bidirectional,
    }
    public enum EffectSequence
    {
        PostEffect,
        PreEffect,
        PreWord,
        PostWord,
    }

    public SpellFxData fx;
    public Direction direction;
    public ModifierWordEffect specialEffect;
    public SpellTag.TagSet tagsToAdd;

    public void Modify(RootWordEffect effect)
    {    
        if (specialEffect != null)
        {
            specialEffect.ApplyEffect(effect, tagsToAdd);
            if(!specialEffect.OverrideTags)
                foreach (var tag in tagsToAdd)
                    effect.tags.Add(tag);
        }
        else
        {
            foreach (var tag in tagsToAdd)
                effect.tags.Add(tag);
        }       
    }
    public void Modify(SpellWord[] words, int index)
    {
        if(direction == Direction.Left || direction == Direction.Bidirectional)
        {
            if (index - 1 >= 0 && words[index - 1] is RootWord)
            {
                var word = words[index - 1] as RootWord;
                foreach (var effect in word.effects)
                    Modify(effect);
                word.modifiers.Add(this);
            }
        }
        if (direction == Direction.Right || direction == Direction.Bidirectional)
        {
            if (index + 1 < words.Length && words[index + 1] is RootWord)
            {
                var word = words[index + 1] as RootWord;
                foreach (var effect in word.effects)
                    Modify(effect);
                word.modifiers.Add(this);
            }
        }
    }

    public override SpellWord Clone()
    {
        var clone = Instantiate(this);
        if(specialEffect != null)
            clone.specialEffect = Instantiate(specialEffect);
        return clone;
    }
}
