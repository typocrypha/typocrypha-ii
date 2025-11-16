using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Prefab")]
public class PrefabBundle : BetterBundle<GameObject>
{
    public PrefabDictionary prefabs;

    public override void Add(GameObject item)
    {
        prefabs[item.name] = item;
    }

    public override void Clear()
    {
        prefabs.Clear();
    }

    [System.Serializable]
    public class PrefabDictionary : SerializableDictionary<string, GameObject> { }
}
