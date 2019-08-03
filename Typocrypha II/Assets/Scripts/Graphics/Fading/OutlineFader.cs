using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that allows sprite renderers to be faded.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class OutlineFader : Fader
{
    SpriteRenderer sr; // Attached sprite renderer.
    Material outlineMaterial; 

    public override float FadeAmount
    {
        get => outlineMaterial.GetFloat("_FadeAmount");
        set => outlineMaterial.SetFloat("_FadeAmount", value);
    }

    public override Color FadeColor
    {
        get => outlineMaterial.GetColor("_FadeColor");
        set => outlineMaterial.SetColor("_FadeColor", value);
    }

    void Awake()
    {
        if (FaderManager.instance == null) return;
        // Add fader to list of all faders.
        FaderManager.instance.allFaders.Add(this);
        // Get outline fader
        outlineMaterial = GetComponentInParent<DialogCharacter>().OutlineMaterial;
    }
}

