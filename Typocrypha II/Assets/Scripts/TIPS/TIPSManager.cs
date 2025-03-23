//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Manages interfacing with TIPS database.
/// </summary>
public class TIPSManager : MonoBehaviour
{
    public static TIPSManager Instance = null;

    private TIPSEntryData currSearchable; // Current dialog line's searchable term.
    public TIPSEntryData CurrSearchable
    {
        get => currSearchable;
        set => currSearchable = value;
    }

    [SerializeField] public TIPSBundle allTIPS;

    public IReadOnlyDictionary<string, TIPSEntryData> UnlockedEntries => unlockedEntries;
    private readonly Dictionary<string, TIPSEntryData> unlockedEntries = new Dictionary<string, TIPSEntryData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public bool EntryExists(string title)
    {
        return allTIPS.entries.ContainsKey(title);
    }

    public TIPSEntryData GetEntry(string title)
    {
        Debug.Assert(EntryExists(title));
        return allTIPS.entries[title];
    }

    public bool TryGetEntry(string title, out TIPSEntryData match)
    {
        return allTIPS.entries.TryGetValue(title, out match);
    }

    public bool EntryIsUnlocked(string title)
    {
        return unlockedEntries.ContainsKey(title);
    }

    /// <summary>
    /// Register entry to collection of unlocked entries.
    /// Will do nothing if entry already unlocked or doesn't exist
    /// </summary>
    /// <param name="title"> Title of entry to be added. </param>
    public void UnlockEntry(string title)
    {
        if (!EntryExists(title)) return;
        if (EntryIsUnlocked(title)) return;
        unlockedEntries.Add(title, allTIPS.entries[title]);
    }

    /// <summary>
    /// Search unlocked entries by title.
    /// </summary>
    /// <param name="titlePartial"> Substring to match title against. </param>
    /// <returns> Array of matching entries. </returns>
    public TIPSEntryData[] GetUnlockedEntriesWithPartialTitle(string titlePartial)
    {
        return unlockedEntries
            .Select(p => p.Value)
            .Where(e => e.MatchTitlePartial(titlePartial))
            .ToArray();
    }

    /// <summary>
    /// Combines the player action of searching and unlocking
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public TIPSEntryData[] HandlePlayerQuery(string query)
    {
        UnlockEntry(query);
        return GetUnlockedEntriesWithPartialTitle(query);
    }

    public TIPSEntryData[] FilterEntriesByPath(string entryPath)
    {
        return allTIPS.entries
            .Select(p => p.Value)
            .Where(e => e.EntryPath == entryPath)
            .ToArray();
    }
}
