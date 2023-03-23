using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName ="RootWord", menuName ="Spell Word/Root")]
public class RootWord : SpellWord
{
    #region Properties when modified (maybe move to distinct class)
    [System.NonSerialized]
    public ModifierWord leftMod;
    [System.NonSerialized]
    public ModifierWord rightMod;
    #endregion

    public List<RootWordEffect> effects = new List<RootWordEffect>();
    public Sprite icon; // Icon displayed when charging up spell.
    public override SpellWord Clone()
    {
        var clone = Instantiate(this);
        clone.effects = new List<RootWordEffect>(effects.Count);
        foreach(var effect in effects)
        {
            clone.effects.Add(Instantiate(effect));
        }
        return clone;
    }
}
