using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that allows sprite renderers to be faded.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFader : Fader
{
    SpriteRenderer sr; // Attached sprite renderer.

    public override float FadeAmount
    {
        get => fadeMat.GetFloat("_FadeAmount");
        set => fadeMat.SetFloat("_FadeAmount", value);
    }

    public override Color FadeColor
    {
        get => fadeMat.GetColor("_FadeColor");
        set => fadeMat.SetColor("_FadeColor", value);
    }

    void Awake()
    {
        // Add fader to list of all faders.
        FaderManager.instance.allFaders.Add(this);
        // Append fade shader to list of shaders.
        sr = GetComponent<SpriteRenderer>();
        Material[] mats = sr.materials;
        Material[] appended = new Material[sr.materials.Length + 1];
        for (int i = 0; i < sr.materials.Length; i++)
        {
            appended[i] = mats[i];
        }
        fadeMat = new Material(fadeShader);
        appended[sr.materials.Length] = fadeMat;
        sr.materials = appended;
    }
}
