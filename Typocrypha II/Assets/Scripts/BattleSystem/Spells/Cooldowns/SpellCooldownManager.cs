using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Unity;

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

    public bool Overheated
    {
        get
        {
            foreach(var kvp in cooldowns)
            {
#if DEBUG
                if (kvp.Value.SpellWord.IsDebug)
                    continue;
#endif
                if (!kvp.Value.OnCooldown)
                    return false;
            }
            return true;
        }
    }

    // Map of spell rootwords to remaining cooldown time.
    //private readonly Dictionary<string, int> cooldowns = new Dictionary<string, int>();
    private readonly Dictionary<string, SpellCooldown> cooldowns = new Dictionary<string, SpellCooldown>();
    public GameObject cooldownPrefab; // Prefab for single cooldown UI object.
    public Transform cooldownTr; // Object that contains all cooldown UI.
    [SerializeField] private OverheatManager overheatManager;

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
        InitializeEquippedWords();
    }

    public void InitializeEquippedWords()
    {
        foreach (var kvp in PlayerDataManager.instance.equipment.EquippedSpellWords)
        {
            AddWord(kvp.Value);
        }
        SortCooldowns();
    }

    public void SortCooldowns()
    {
        if (cooldowns.Count <= 0)
            return;
        cooldownTr.SortHierarchy(CompareCooldownsHeirarchy);
    }

    public IReadOnlyList<SpellWord> GetSpells()
    {
        var ret = new List<SpellWord>(cooldowns.Count);
        var cooldownList = new List<SpellCooldown>(cooldowns.Values);
        cooldownList.Sort(CompareCooldowns);
        foreach(var cooldown in cooldownList)
        {
            if (cooldown != null)
            {
                ret.Add(cooldown.SpellWord);
            }
        }
        return ret;
    }

    public IReadOnlyDictionary<string, SpellWord> GetSpellsDict()
    {
        var ret = new Dictionary<string, SpellWord>(cooldowns.Count);
        foreach (var kvp in cooldowns)
        {
            var cooldown = kvp.Value;
            if (cooldown != null)
            {
                ret.Add(cooldown.SpellWord.Key, cooldown.SpellWord);
            }
        }
        return ret;
    }

    private int CompareCooldownsHeirarchy(Transform a, Transform b)
    {
        return -CompareCooldowns(a.GetComponent<SpellCooldown>(), b.GetComponent<SpellCooldown>());
    }

    private int CompareCooldowns(SpellCooldown aCooldown, SpellCooldown bCooldown)
    {
        if (bCooldown.IsFixedUse)
        {
            if (!aCooldown.IsFixedUse)
                return ComparisonConstants.greaterThan;
            if (bCooldown.Uses == aCooldown.Uses)
                return aCooldown.SpellText.CompareTo(bCooldown.SpellText);
            return aCooldown.Uses.CompareTo(bCooldown.Uses);
        }
        if (aCooldown.IsFixedUse)
        {
            return ComparisonConstants.lessThan;
        }
        if (bCooldown.Cooldown == aCooldown.Cooldown)
        {
            if (bCooldown.SpellWord.category == aCooldown.SpellWord.category)
            {
                if (bCooldown.FullCooldown == aCooldown.FullCooldown)
                {
                    return aCooldown.SpellText.CompareTo(bCooldown.SpellText);
                }
                return aCooldown.FullCooldown.CompareTo(bCooldown.FullCooldown);
            }
            return aCooldown.SpellWord.category.CompareTo(bCooldown.SpellWord.category);
        }
        return aCooldown.Cooldown.CompareTo(bCooldown.Cooldown);
    }

    public void ClearWords()
    {
        foreach(var kvp in cooldowns)
        {
            Destroy(kvp.Value.gameObject);
        }
        cooldowns.Clear();
    }

    public void AddWord(SpellWord word, bool sort = false)
    {
        // If cooldown for this word already exists, return
        if (TryGetCooldown(word, out _))
            return;
        var cd = NewCooldown(word);
        cd.SetupCooldown(word.cooldown);
        AddNewCooldown(cd, sort);
    }

    public void AddFixedUseWord(SpellWord word, int uses, int maxUses, bool sort = false)
    {
        // If cooldown for this word already exists, return
        if (TryGetCooldown(word, out var existingCd))
        {
            if (existingCd.IsFixedUse)
            {
                existingCd.Uses += uses;
            }
            return;
        }
        var cd = NewCooldown(word);
        cd.SetupWithFixedUses(uses, maxUses);
        AddNewCooldown(cd, sort);
    }

    private SpellCooldown NewCooldown(SpellWord word)
    {
        var cd = Instantiate(cooldownPrefab, cooldownTr).GetComponent<SpellCooldown>();
        cd.SpellText = word.internalName.ToUpper();
        cd.SpellWord = word;
        return cd;
    }

    private void AddNewCooldown(SpellCooldown cd, bool sort)
    {
        cooldowns.Add(cd.SpellWord.internalName.ToUpper(), cd);
        if (sort)
        {
            SortCooldowns();
        }
    }

    public void RemoveWord(SpellWord word)
    {
        string key = word.internalName.ToUpper();
        // If cooldown for this word doesn't exist, return
        if (!cooldowns.ContainsKey(key))
            return;
        Destroy(cooldowns[key].gameObject);
        cooldowns.Remove(key);
    }

    public bool IsOnCooldown(SpellWord word)
    {
        return TryGetCooldown(word, out var cooldown) && cooldown.OnCooldown;
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
        // Lower all cooldowns by num roots
        LowerAllCooldowns(spell.RootCount); 
        // Add new cooldowns
        foreach (var word in spell)
        {
            if (TryGetCooldown(word, out var cooldown))
            {
                int totalCooldown = word.cooldown;
                if(Rule.ActiveRule != null)
                {
                    totalCooldown += Rule.ActiveRule.CooldownModifier(word);
                }
                if (cooldown.OnCast(totalCooldown))
                {
                    RemoveWord(cooldown.SpellWord);
                }
            }
        }
        SortCooldowns();
    }

    public void LowerAllCooldowns(int amount)
    {
        foreach (var kvp in cooldowns)
        {
            kvp.Value.LowerCooldown(amount); 
        }
    }

    private bool TryGetCooldown(SpellWord word, out SpellCooldown cooldown)
    {
        if(word.synonymOf != null)
        {
            return cooldowns.TryGetValue(word.synonymOf.internalName.ToUpper(), out cooldown);
        }
        return cooldowns.TryGetValue(word.internalName.ToUpper(), out cooldown);
    }

    public void ResetAllCooldowns()
    {
        foreach(var kvp in cooldowns)
        {
            kvp.Value.ResetCooldown();
        }
        SortCooldowns();
    }

    public void DoOverheat()
    {
        overheatManager.DoOverheat();
    }

    public void StopOverheat()
    {
        overheatManager.StopOverheat();
    }
}
