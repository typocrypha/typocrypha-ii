using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of a single spell cooldown. Mostly controls UI.
/// </summary>
public class SpellCooldown : MonoBehaviour
{
    public UnityEvent_int OnChangeTotalCooldown; // Handle called when total cooldown is set.
    public UnityEvent_int OnChangeCurrCooldown; // Handle called when cooldown changes (amount remaining).
    public Text spellText; // Text for spell name.
    int totalCooldown; // Total cooldown.
    public int TotalCooldown
    {
        get => totalCooldown;
        set
        {
            totalCooldown = value;
            OnChangeTotalCooldown.Invoke(value);
        }
    }
    int currCooldown; // Current cooldown remaining.
    public int CurrCooldown
    {
        get => currCooldown;
        set
        {
            currCooldown = value;
            OnChangeCurrCooldown.Invoke(value);
            SpellCooldownManager.instance.SortCooldowns();
        }
    }

    /// <summary>
    /// Start cooldown for word.
    /// </summary>
    public void StartCooldown()
    {
        CurrCooldown = TotalCooldown;
    }
}
