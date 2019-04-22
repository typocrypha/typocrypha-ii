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

    public float cooldown; // Base cooldown time

    public List<RootWordEffect> effects = new List<RootWordEffect>();
    public override SpellWord Clone()
    {
        var clone = Instantiate(this);
        clone.effects = effects.Select((effect) => (Instantiate(effect))).ToList();
        return clone;
    }
}
