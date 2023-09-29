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
                if (kvp.Value.Cooldown <= 0)
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
        foreach (var kvp in PlayerDataManager.instance.equipment.EquippedWords)
        {
            AddWord(kvp.Value);
        }
        SortCooldowns();
    }

    public void SortCooldowns()
    {
        if (cooldowns.Count <= 0)
            return;
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
                cooldown.Cooldown += word.cooldown;
            }
        }
        SortCooldowns();
    }

    public void LowerAllCooldowns(int amount)
    {
        foreach (var kvp in cooldowns)
        {
            kvp.Value.Cooldown -= amount; 
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
            kvp.Value.Cooldown = 0;
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
