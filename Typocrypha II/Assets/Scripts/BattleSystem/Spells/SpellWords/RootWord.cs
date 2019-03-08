using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RootWord", menuName ="Spell Word/Root")]
public class RootWord : SpellWord
{
    [System.NonSerialized] public List<ModifierWord> modifiers = new List<ModifierWord>();
    public List<RootWordEffect> effects = new List<RootWordEffect>();
}
