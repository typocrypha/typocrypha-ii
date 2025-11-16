using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Badge")]
public class BadgeBundle : BetterBundle<BadgeWord>
{
    public BadgeDictionary badges;

    public override void Add(BadgeWord item)
    {
        badges[item.Key] = item;
    }

    public override void Clear()
    {
        badges.Clear();
    }

    [System.Serializable] public class BadgeDictionary : SerializableDictionary<string, BadgeWord> { };
}
