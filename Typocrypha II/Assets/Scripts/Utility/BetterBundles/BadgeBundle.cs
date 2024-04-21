using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Badge")]
public class BadgeBundle : BetterBundle
{
    public BadgeDictionary badges;
    [System.Serializable] public class BadgeDictionary : SerializableDictionary<string, BadgeWord> { };
}
