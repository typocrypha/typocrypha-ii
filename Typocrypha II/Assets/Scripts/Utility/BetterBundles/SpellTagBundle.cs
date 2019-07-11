using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/SpellTag")]
public class SpellTagBundle : BetterBundle
{
    public TagDictionary tags;

    [System.Serializable]
    public class TagDictionary : SerializableDictionary<string, SpellTag> { }
}
