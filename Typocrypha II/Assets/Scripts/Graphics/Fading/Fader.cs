using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component that allows objects to be faded.
/// </summary>
public abstract class Fader : MonoBehaviour
{
    public Shader fadeShader; // Shader that allows for fading.
    protected Material fadeMat; // Created material for fading.

    /// <summary>
    /// Set/get the fade amount (goes from 0 to 1).
    /// 0: No fade. 1: Total fade.
    /// </summary>
    public abstract float FadeAmount
    {
        get;
        set;
    } 

    /// <summary>
    /// Set/get the fade color.
    /// </summary>
    public abstract Color FadeColor
    {
        get;
        set;
    }
}
