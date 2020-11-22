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
    public const int invalidCooldown = -1;
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
        var equipment = PlayerEquipment.instance.EquippedWords;
        foreach (var spell in equipment)
        {
            var cd = Instantiate(cooldownPrefab, cooldownTr).GetComponent<SpellCooldown>();
            cd.spellText.text = spell.internalName.ToUpper();
            cd.TotalCooldown = spell.cooldown;
            cd.CurrCooldown = 0;
            cooldowns.Add(spell.internalName.ToUpper(), cd);
        }
    }

    /// <summary>
    /// Start cooldown timer for given spell.
    /// If the spell is already on cooldown, return and do nothing
    /// </summary>
    /// <param name="word">Name of spell to put on cooldown</param>
    public void StartCooldown(string word)
    {
        word = word.ToUpper();
        if (IsOnCooldown(word))
            return;
        cooldowns[word].StartCooldown();
    }

    /// <summary>
    /// Returns true if the word is on cooldown, else false
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public bool IsOnCooldown(string word)
    {
        word = word.ToUpper();
        if (cooldowns.ContainsKey(word))
        {
            return cooldowns[word].CurrCooldown > 0;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the number of uses left on this word's cooldowns if the word is on cooldown.
    /// Throws exception if the word is not on cooldown.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    int GetCooldown(string word)
    {
        if (cooldowns.ContainsKey(word))
        {
            return cooldowns[word].CurrCooldown;
        }
        else
        {
            throw new UnityException("Invalid Cooldown");
        }
    }

    /// <summary>
    /// Changes the cooldown amount by the number in modifier.
    /// If the cooldown reaches 0, the spell is taken off of cooldown.
    /// If mustAlreadyBeOnCooldown is false and the modifier >= t, the spell will be put on cooldown
    /// </summary>
    /// <param name="word"></param>
    /// <param name="modifier"></param>
    void ModifyCooldown(string word, int modifier, bool mustAlreadyBeOnCooldown = true)
    {
        if(!IsOnCooldown(word))
        {
            if (mustAlreadyBeOnCooldown || modifier <= 0)
                return;
            StartCooldown(word);
        }
        else if(cooldowns[word].CurrCooldown + modifier <= 0)
        {
            ResetCooldown(word);
        }
        else
        {
            cooldowns[word].CurrCooldown += modifier;
        }
    }

    public void ModifyAllCooldowns(int modifier, bool mustAlreadyBeOnCooldown = true)
    {
        foreach (var word in cooldowns.Keys.ToArray())
            ModifyCooldown(word, modifier, mustAlreadyBeOnCooldown);
    }

    /// <summary>
    /// Reset the cooldown for the given spell.
    /// </summary>
    /// <param name="word">Name of spell to reset cooldown.</param>
    public void ResetCooldown(string word)
    {
        word = word.ToUpper();
        if (IsOnCooldown(word))
        {
            cooldowns[word].CurrCooldown = 0;
        }
    }
    /// <summary>
    /// Resets the cooldown for all spells to 0
    /// </summary>
    public void ResetAllCooldowns()
    {
        foreach(var spell in cooldowns.Keys)
        {
            cooldowns[spell].CurrCooldown = 0;
        }
    }
}
