using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Caster")]
public class CasterBundle : BetterBundle<GameObject>
{
    public CasterDictionary prefabs;

    public override void Add(GameObject item)
    {
        var caster = item.GetComponent<Caster>();
        if (caster == null || string.IsNullOrEmpty(caster.DisplayName))
            return;
        if (prefabs.ContainsKey(caster.DisplayName))
            return;
        prefabs[caster.DisplayName] = item;
    }

    public override void Clear()
    {
        prefabs.Clear();
    }

    [System.Serializable]
    public class CasterDictionary : SerializableDictionary<string, GameObject> { }
}
