using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/TIPS")]
public class TIPSBundle : BetterBundle
{
    public TIPSDictionary entries;

    [System.Serializable] 
    public class TIPSDictionary : SerializableDictionary<string, TIPSEntryData> 
    {
        protected override void InitializeDictionary()
        {
            _dictionary = new Dictionary<string, TIPSEntryData>(System.StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
