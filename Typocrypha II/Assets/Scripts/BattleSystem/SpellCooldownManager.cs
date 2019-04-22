using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of spell cooldowns.
/// </summary>
public class SpellCooldownManager : MonoBehaviour
{
    public static SpellCooldownManager instance = null;
    // Map of spell rootwords to remaining cooldown time.
    public Dictionary<string, float> cooldowns = new Dictionary<string, float>();
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
    }

    /// <summary>
    /// Start cooldown timer for given spell.
    /// </summary>
    /// <param name="spell">Name of spell cooling down.</param>
    /// <param name="time">Time to cooldown.</param>
    public void StartCooldown(string spell, float time)
    {
        cooldowns[spell] = time;
        StartCoroutine(CooldownCR(spell));
    }

    /// <summary>
    /// Reset the cooldown for the given spell.
    /// </summary>
    /// <param name="spell">Name of spell to reset cooldown.</param>
    public void ResetCooldown(string spell)
    {
        if (cooldowns.ContainsKey(spell))
            cooldowns[spell] = 0f;
    }

    /// <summary>
    /// Reset all spell cooldowns.
    /// </summary>
    public void ResetAll()
    {
        foreach (var kvp in cooldowns)
            cooldowns[kvp.Key] = 0f;
    }

    // Cooldown timer. Waits for 'time' seconds.
    IEnumerator CooldownCR(string spell)
    {
        SpellCooldown cd = Instantiate(cooldownPrefab, cooldownTr).GetComponent<SpellCooldown>();
        cd.spellText.text = spell;
        cd.TotalTime = cooldowns[spell];
        while (cooldowns[spell] > 0f)
        {
            cooldowns[spell] -= Time.fixedDeltaTime;
            cd.CurrTime = cooldowns[spell];
            yield return new WaitForFixedUpdate();
        }
        cooldowns[spell] = 0f;
        Destroy(cd.gameObject);
    }
}
