using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of spell cooldowns.
/// </summary>
public class SpellCooldownManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }
    public void OnPause(bool b)
    {

    }
    #endregion
    public static SpellCooldownManager instance = null;
    // Map of spell rootwords to remaining cooldown time.
    //private readonly Dictionary<string, int> cooldowns = new Dictionary<string, int>();
    private readonly Dictionary<string, SpellCooldown> cooldowns = new Dictionary<string, SpellCooldown>();
    public GameObject cooldownPrefab; // Prefab for single cooldown UI object.
    public Transform cooldownTr; // Object that contains all cooldown UI.

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        ph = new PauseHandle(OnPause);
    }

    private void Start()
    {
        var equipment = PlayerDataManager.instance.equipment.EquippedWords;
        foreach (var spell in equipment)
        {
            AddWord(spell);
        }
        SortCooldowns();
    }

    public void SortCooldowns()
    {       
        cooldownTr.SortHiearchy(CompareCooldowns);
    }

    private int CompareCooldowns(Transform a, Transform b)
    {
        var aCooldown = a.GetComponent<SpellCooldown>();
        var bCooldown = b.GetComponent<SpellCooldown>();
        if(aCooldown.Cooldown == bCooldown.Cooldown)
        {
            if(aCooldown.SpellWord.category == bCooldown.SpellWord.category)
            {
                if (aCooldown.FullCooldown == bCooldown.FullCooldown)
                {
                    return bCooldown.SpellText.CompareTo(aCooldown.SpellText);
                }
                return bCooldown.FullCooldown.CompareTo(aCooldown.FullCooldown);
            }
            return bCooldown.SpellWord.category.CompareTo(aCooldown.SpellWord.category);
        }
        return bCooldown.Cooldown.CompareTo(aCooldown.Cooldown);
    }

    public void AddWord(SpellWord word, bool sort = false)
    {
        if (IsOnCooldown(word))
            return;
        var cd = Instantiate(cooldownPrefab, cooldownTr).GetComponent<SpellCooldown>();
        cd.SpellText = word.internalName.ToUpper();
        cd.SpellWord = word;
        cd.FullCooldown = word.cooldown;
        cd.Cooldown = 0;
        cooldowns.Add(word.internalName.ToUpper(), cd);
        if (sort)
        {
            SortCooldowns();
        }
    }

    public bool IsOnCooldown(SpellWord word)
    {
        return TryGetCooldown(word, out var cooldown) && cooldown.Cooldown > 0;
    }

    public bool IsOnCooldown(string word)
    {
        return TryGetCooldown(word, out var cooldown) && cooldown.Cooldown > 0;
    }

    public bool IsOnCooldown(Spell spell, out SpellWord word)
    {
        foreach (var item in spell)
        {
            if (IsOnCooldown(item))
            {
                word = item;
                return true;
            }
        }
        word = null;
        return false;
    }

    public void DoCooldowns(Spell spell)
    {
        var rootCounts = spell.RootCounts;
        foreach (var kvp in cooldowns)
        {
            kvp.Value.Cooldown -= rootCounts.Count; // Unique roots
        }
        foreach (var kvp in rootCounts)
        {
            if (TryGetCooldown(kvp.Key, out var cooldown))
            {
                cooldown.Cooldown = cooldown.FullCooldown + (kvp.Value - 1);
            }
        }
        SortCooldowns();
    }

    private bool TryGetCooldown(string word, out SpellCooldown cooldown)
    {
        return cooldowns.TryGetValue(word, out cooldown);
    }

    private bool TryGetCooldown(SpellWord word, out SpellCooldown cooldown)
    {
        return cooldowns.TryGetValue(word.internalName.ToUpper(), out cooldown);
    }

    public void ResetAllCooldowns()
    {
        foreach(var kvp in cooldowns)
        {
            kvp.Value.Cooldown = 0;
        }
        SortCooldowns();
    }
}
