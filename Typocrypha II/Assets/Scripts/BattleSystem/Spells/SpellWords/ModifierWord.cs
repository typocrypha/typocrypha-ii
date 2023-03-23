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
    [SubSO("Sp. Effect")]
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
    public void Modify(IList<SpellWord> words, int index)
    {
        if(direction == Direction.Left || direction == Direction.Bidirectional)
        {
            if (index - 1 >= 0 && words[index - 1] is RootWord root)
            {
                foreach (var effect in root.effects)
                    Modify(effect);
                root.rightMod = this;
            }
        }
        if (direction == Direction.Right || direction == Direction.Bidirectional)
        {
            if (index + 1 < words.Count && words[index + 1] is RootWord root)
            {
                foreach (var effect in root.effects)
                    Modify(effect);
                root.leftMod = this;
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
