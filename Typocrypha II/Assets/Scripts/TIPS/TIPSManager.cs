using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Manages interfacing with TIPS database.
/// </summary>
public class TIPSManager : MonoBehaviour
{
    public static TIPSManager Instance = null;
    public PauseHandle PH { get; private set; } = null;

    private TIPSEntryData currSearchable; // Current dialog line's searchable term.
    public TIPSEntryData CurrSearchable
    {
        get => currSearchable;
        set => currSearchable = value;
    }

    [SerializeField] public TIPSBundle allTIPS;

    public IReadOnlyDictionary<string, TIPSEntryData> UnlockedEntries => unlockedEntries;
    private readonly TIPSBundle.TIPSDictionary unlockedEntries = new TIPSBundle.TIPSDictionary();
    
    private GameObject lastSelected;
    private EventSystem currentEventSystem;

    private void Awake()
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
        currentEventSystem = EventSystem.current;
        PH = new PauseHandle(OnPause);
    }

    private void OnPause(bool pause)
    {
        if (pause)
        {
            var currentSelected = currentEventSystem.currentSelectedGameObject;
            if (currentSelected.layer == LayerMask.NameToLayer("TIPS"))
            {
                lastSelected = currentSelected;
                currentEventSystem.SetSelectedGameObject(null);
            }
        }
        else
        {
            if (lastSelected != null) currentEventSystem.SetSelectedGameObject(lastSelected);
            lastSelected = null;
        }
    }

    public bool EntryExists(string id)
    {
        return allTIPS.entries.ContainsKey(id);
    }

    public TIPSEntryData GetEntry(string id)
    {
        return EntryExists(id) ? allTIPS.entries[id] : null;
    }

    public bool TryGetEntry(string id, out TIPSEntryData match)
    {
        return allTIPS.entries.TryGetValue(id, out match);
    }

    public bool EntryIsUnlocked(string id)
    {
        return unlockedEntries.ContainsKey(id);
    }

    /// <summary>
    /// Register entry to collection of unlocked entries.
    /// Will do nothing if entry already unlocked or doesn't exist
    /// </summary>
    /// <param name="id"> Title of entry to be added. </param>
    public void UnlockEntryIfApplicable(string id)
    {
        if (!EntryExists(id)) return;
        if (EntryIsUnlocked(id)) return;
        unlockedEntries.Add(allTIPS.entries[id].ID, allTIPS.entries[id]);
        UnlockEntryIfApplicable(allTIPS.entries[id].Parent);
    }

    /// <summary>
    /// Search unlocked entries by title.
    /// </summary>
    /// <param name="idPartial"> Substring to match title against. </param>
    /// <returns> Array of matching entries. </returns>
    public TIPSEntryData[] GetUnlockedEntriesWithPartialTitle(string idPartial)
    {
        return unlockedEntries
            .Select(p => p.Value)
            .Where(e => e.MatchTitlePartial(idPartial))
            .ToArray();
    }
    
    /// <summary>
     /// Search unlocked entries by alias.
     /// </summary>
     /// <param name="query"> String to match alias against. </param>
     /// <returns> Array of matching entries. </returns>
    public TIPSEntryData GetUnlockedEntryWithExactAlias(string query)
    {
        return unlockedEntries
            .Select(p => p.Value)
            .Where(e => e.MatchAliasExact(query))
            .FirstOrDefault();
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
        var aliasMatch = GetUnlockedEntryWithExactAlias(query);
        partialMatches = GetUnlockedEntriesWithPartialTitle(query);

        if (aliasMatch) return aliasMatch;
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

    public void PauseAllForTIPS(bool pause)
    {
        PauseManager.instance.PauseAll(false, PauseSources.TIPS, PH, false);
    }
}
