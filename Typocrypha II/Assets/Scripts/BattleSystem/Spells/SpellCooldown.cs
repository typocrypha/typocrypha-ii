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
    public float TotalTime { get; set; } // Total cooldown time.
    float currTime;
    public float CurrTime
    {
        get => currTime;
        set
        {
            currTime = value;
            onChangeTimeString.Invoke(CurrTime.ToString("R"));
            onChangeTimeRatio.Invoke(CurrTime/TotalTime);
        }
    }
}
