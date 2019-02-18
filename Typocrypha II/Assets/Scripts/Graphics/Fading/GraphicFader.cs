using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component that allows ui graphics to be faded.
/// e.g. Text, Image, etc.
/// </summary>
[RequireComponent(typeof(Graphic))]
public class GraphicFader : Fader
{
    Graphic graphic; // Attached graphic.

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
        // Add fade material.
        graphic = GetComponent<Graphic>();
        fadeMat = new Material(fadeShader);
        graphic.material = fadeMat;
    }
}
