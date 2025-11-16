using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/SpellTag")]
public class SpellTagBundle : BetterBundle<SpellTag>
{
    public TagDictionary tags;

    public override void Add(SpellTag item)
    {
        tags[item.internalName] = item;
    }

    public override void Clear()
    {
        tags.Clear();
    }

    [System.Serializable]
    public class TagDictionary : SerializableDictionary<string, SpellTag> { }
}
