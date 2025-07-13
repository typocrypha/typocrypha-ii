using UnityEngine;
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
    private readonly TIPSBundle.TIPSDictionary unlockedEntries = new TIPSBundle.TIPSDictionary();

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
        return EntryExists(title) ? allTIPS.entries[title] : null;
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
    public void UnlockEntryIfApplicable(string title)
    {
        if (!EntryExists(title)) return;
        if (EntryIsUnlocked(title)) return;
        unlockedEntries.Add(allTIPS.entries[title].Title, allTIPS.entries[title]);
        UnlockEntryIfApplicable(allTIPS.entries[title].Parent);
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
    /// Combines the player action of entry lookup and unlocking.
    /// </summary>
    /// <param name="query"> Title search term. </param>
    /// <param name="partialMatches"> Entries matching query. </param>
    /// <returns> Matching entry or null. </returns>
    public TIPSEntryData HandlePlayerQuery(string query, out TIPSEntryData[] partialMatches)
    {
        UnlockEntryIfApplicable(query);
        partialMatches = GetUnlockedEntriesWithPartialTitle(query);
        return partialMatches.FirstOrDefault(e => e.MatchTitleExact(query));
    }

    public TIPSEntryData[] FilterEntries(string parent, bool unlockedOnly)
    {
        var tipsDictionary = unlockedOnly ? unlockedEntries : allTIPS.entries;
        return tipsDictionary
            .Select(p => p.Value)
            .Where(e => e.Parent == parent)
            .ToArray();
    }
}
