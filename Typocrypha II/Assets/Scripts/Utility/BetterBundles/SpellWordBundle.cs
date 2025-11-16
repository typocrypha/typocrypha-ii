using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/SpellWord")]
public class SpellWordBundle : BetterBundle<SpellWord>
{
    public SpellWordDictionary words;

    public override void Add(SpellWord item)
    {
        words[item.Key] = item;
    }

    public override void Clear()
    {
        words.Clear();
    }

    [System.Serializable] public class SpellWordDictionary : SerializableDictionary<string, SpellWord> { };
}
