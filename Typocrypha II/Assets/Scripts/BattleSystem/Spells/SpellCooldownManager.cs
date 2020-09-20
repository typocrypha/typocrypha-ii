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
    private readonly Dictionary<string, int> cooldowns = new Dictionary<string, int>();
    private readonly Dictionary<string, SpellCooldown> cooldownUI = new Dictionary<string, SpellCooldown>();
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

    /// <summary>
    /// Start cooldown timer for given spell.
    /// If the spell is already on cooldown, return and do nothing
    /// </summary>
    /// <param name="word">Name of spell to put on cooldown</param>
    /// <param name="uses">Uses to cooldown.</param>
    public void StartCooldown(string word, int uses)
    {
        if (IsOnCooldown(word))
            return;
        cooldowns.Add(word, uses);
        // Instiantiate new cooldown UI
        var cd = Instantiate(cooldownPrefab, cooldownTr).GetComponent<SpellCooldown>();
        cd.spellText.text = word.ToUpper();
        cd.TotalUses = cooldowns[word];
        cd.CurrUses = cd.TotalUses;
        cooldownUI.Add(word, cd);
    }

    /// <summary>
    /// Returns true if the word is on cooldown, else false
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public bool IsOnCooldown(string word)
    {
        return cooldowns.ContainsKey(word);
    }

    /// <summary>
    /// Returns the number of uses left on this word's cooldowns if the word is on cooldown.
    /// returns SpellCooldownManager.invalidCooldown if the word is not on cooldown.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public int GetCooldown(string word)
    {
        if (cooldowns.ContainsKey(word))
            return cooldowns[word];
        return invalidCooldown;
    }

    /// <summary>
    /// Changes the cooldown amount by the number in modifier.
    /// If the cooldown reaches 0, the spell is taken off of cooldown.
    /// If mustAlreadyBeOnCooldown is false and the modifier >= t, the spell will be put on cooldown
    /// </summary>
    /// <param name="word"></param>
    /// <param name="modifier"></param>
    public void ModifyCooldown(string word, int modifier, bool mustAlreadyBeOnCooldown = true)
    {
        if(!IsOnCooldown(word))
        {
            if (mustAlreadyBeOnCooldown || modifier <= 0)
                return;
            StartCooldown(word, modifier);
        }
        else if(cooldowns[word] + modifier <= 0)
        {
            RemoveCooldown(word);
        }
        else
        {
            cooldowns[word] += modifier;
            var cd = cooldownUI[word];
            cd.TotalUses = Mathf.Max(cooldowns[word], cd.TotalUses);
            cd.CurrUses = cooldowns[word];
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
    public void RemoveCooldown(string word)
    {
        if (IsOnCooldown(word))
        {
            cooldowns.Remove(word);
            var cd = cooldownUI[word];
            Destroy(cd.gameObject);
            cooldownUI.Remove(word);
        }
    }

    /// <summary>
    /// Reset all spell cooldowns.
    /// </summary>
    public void RemoveAllCooldowns()
    {
        foreach (var word in cooldowns.Keys.ToArray())
            RemoveCooldown(word);
    }
}
