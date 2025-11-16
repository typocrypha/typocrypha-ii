using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/TIPS")]
public class TIPSBundle : BetterBundle<TIPSEntryData>
{
    public TIPSDictionary entries;

    public override void Add(TIPSEntryData item)
    {
        entries[item.ID] = item;
    }

    public override void Clear()
    {
        entries.Clear();
    }

    [System.Serializable] public class TIPSDictionary : SerializableDictionary<string, TIPSEntryData> 
    {
        protected override void InitializeDictionary()
        {
            _dictionary = new Dictionary<string, TIPSEntryData>(System.StringComparer.InvariantCultureIgnoreCase);
        }
    };
}
