using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Prefab")]
public class PrefabBundle : BetterBundle
{
    public PrefabDictionary prefabs;

    [System.Serializable]
    public class PrefabDictionary : SerializableDictionary<string, GameObject> { }
}
