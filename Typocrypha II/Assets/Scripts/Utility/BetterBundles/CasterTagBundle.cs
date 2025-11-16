using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/CasterTag")]
public class CasterTagBundle : BetterBundle<CasterTag>
{
    public TagDictionary tags;

    public override void Add(CasterTag item)
    {
        tags[item.internalName] = item;
    }

    public override void Clear()
    {
        tags.Clear();
    }

    [System.Serializable]
    public class TagDictionary : SerializableDictionary<string, CasterTag> { }
}
