using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName ="RootWord", menuName ="Spell Word/Root")]
public class RootWord : SpellWord
{
    [System.NonSerialized] public List<ModifierWord> modifiers = new List<ModifierWord>();
    public List<RootWordEffect> effects = new List<RootWordEffect>();
    public override SpellWord Clone()
    {
        var clone = Instantiate(this);
        clone.effects = effects.Select((effect) => Instantiate(effect)).ToList();
        return clone;
    }
}
