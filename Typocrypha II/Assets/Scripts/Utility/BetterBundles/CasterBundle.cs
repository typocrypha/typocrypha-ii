using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Caster")]
public class CasterBundle : BetterBundle
{
    public CasterDictionary prefabs;

    [System.Serializable]
    public class CasterDictionary : SerializableDictionary<string, GameObject> { }
}
