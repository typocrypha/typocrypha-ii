using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of a single spell cooldown. Mostly controls UI.
/// </summary>
public class SpellCooldown : MonoBehaviour
{
    public UnityEvent_string onChangeTimeString; // Handle called when cooldown time left changes (called with formatted time string).
    public UnityEvent_float onChangeTimeRatio; // Handle called when cooldown time left changes (called with ratio amount).
    public Text spellText; // Text for spell name.
    public int TotalUses { get; set; } // Total cooldown time.
    int currTime;
    public int CurrUses
    {
        get => currTime;
        set
        {
            currTime = value;
            onChangeTimeString.Invoke(CurrUses.ToString());
            onChangeTimeRatio.Invoke(CurrUses/TotalUses);
        }
    }
}
