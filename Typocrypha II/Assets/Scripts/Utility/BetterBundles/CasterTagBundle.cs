using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/CasterTag")]
public class CasterTagBundle : BetterBundle
{
    public TagDictionary tags;

    [System.Serializable]
    public class TagDictionary : SerializableDictionary<string, CasterTag> { }
}
