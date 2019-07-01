using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/SpellWord")]
public class SpellWordBundle : BetterBundle
{
    public SpellWordDictionary words;
    [System.Serializable] public class SpellWordDictionary : SerializableDictionary<string, SpellWord> { };
}
