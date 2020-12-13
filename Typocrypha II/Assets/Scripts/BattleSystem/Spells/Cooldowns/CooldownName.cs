using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages highlighting for spell cooldwon name display.
/// </summary>
public class CooldownName : MonoBehaviour
{
    public Color OnColor; // Color when on cooldown;
    public Color OffColor; // Color when off cooldown;
    public UnityEngine.UI.Text TextDisplay;

    public void SetCooldown(int curr)
    {
        if (curr == 0)
            TextDisplay.color = OffColor;
        else
            TextDisplay.color = OnColor;
    }
}
