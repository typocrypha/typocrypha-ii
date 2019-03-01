using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Root Word", menuName ="Spell Word/Root")]
public class RootWord : SpellWord
{
    public List<RootWordEffect> effects = new List<RootWordEffect>();
}
