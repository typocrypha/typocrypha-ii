using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Groups together faders for objects with multiple renderers.
/// </summary>
public class FaderGroup : MonoBehaviour
{
    public bool searchChildren = false; // Should fader group just init with child faders?
    public List<Fader> faders; // List of faders in group.

    public void Awake()
    {
        if (searchChildren)
        {
            faders = new List<Fader>();
            faders.AddRange(GetComponentsInChildren<Fader>());
        }
    }

    /// <summary>
    /// Set fade amount of all faders in group.
    /// </summary>
    public float FadeAmount
    {
        set
        {
            foreach(Fader f in faders)
            {
                f.FadeAmount = value;
            }
        }
    }

    /// <summary>
    /// Set fade color of all faders in group.
    /// </summary>
    public Color FadeColor
    {
        set
        {
            foreach (Fader f in faders)
            {
                f.FadeColor = value;
            }
        }
    }
}
