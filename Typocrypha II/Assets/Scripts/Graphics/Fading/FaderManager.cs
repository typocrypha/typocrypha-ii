﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Manages all faders in the scene.
/// </summary>
public class FaderManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
    }
    #endregion

    public bool IsFadingScreen { get; private set; }
    public Color ScreenFadeColor => ScreenFader.color;

    public static FaderManager instance = null;
    public List<Fader> allFaders; // List of all faders. Fader instances add themselves.
    [SerializeField] private Image ScreenFader; // Image for fading entire screen.
    [SerializeField] private Canvas canvas;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        ph = new PauseHandle(OnPause);
        allFaders = new List<Fader>();
    }

    /// <summary>
    /// Fade all of the faders in the scene.
    /// </summary>
    /// <param name="fadeAmount">Amount of fade.</param>
    /// <param name="fadeColor">Color of fade.</param>
    public void FadeAll(float fadeAmount, Color fadeColor)
    {
        foreach(Fader f in allFaders)
        {
            f.FadeAmount = fadeAmount;
            f.FadeColor = fadeColor;
        }
    }

    /// <summary>
    /// Fade all except one, which is completely unfaded.
    /// </summary>
    /// <param name="solo">Fader to unfade.</param>
    /// <param name="fadeAmount">Amount of fade for everything else.</param>
    /// <param name="fadeColor">Color of fade for everything else.</param>
    public void Solo(Fader solo, float fadeAmount, Color fadeColor)
    {
        foreach (Fader f in allFaders)
        {
            f.FadeAmount = fadeAmount;
            f.FadeColor = fadeColor;
        }
        solo.FadeAmount = 0;
    }

    /// <summary>
    /// Fade all except one group, which is completely unfaded.
    /// </summary>
    /// <param name="solo">Fader to unfade.</param>
    /// <param name="fadeAmount">Amount of fade for everything else.</param>
    /// <param name="fadeColor">Color of fade for everything else.</param>
    public void Solo(FaderGroup solo, float fadeAmount, Color fadeColor)
    {
        foreach (Fader f in allFaders)
        {
            f.FadeAmount = fadeAmount;
            f.FadeColor = fadeColor;
        }
        solo.FadeAmount = 0;
    }

    /// <summary>
    /// Highlights targets of a spell, fading everything else.
    /// </summary>
    /// <param name="spell">Casted spell.</param>
    /// <param name="fieldPos">Caster's position.</param>
    /// <param name="targetPos">Caster's target position.</param>
    public void FadeTargets(Spell spell, Battlefield.Position fieldPos, Battlefield.Position targetPos)
    {
        FadeAll(0.5f, Color.black);
        foreach (var target in spell.AllTargets(fieldPos, targetPos).Where((a) => a != null))
        {
            var fader = target.GetComponent<FaderGroup>();
            if(fader != null)
            {
                fader.FadeAmount = 0f;
                fader.FadeColor = Color.black;
            }
        }
    }

    /// <summary>
    /// Fade the entire screen.
    /// </summary>
    /// <param name="fadeAmount">Amount of fade.</param>
    /// <param name="fadeColor">Color of fade.</param>
    public void FadeScreen(float fadeAmount, Color fadeColor)
    {
        ScreenFader.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeAmount);
        bool canvasOn = fadeAmount > 0;
        if (canvas.enabled != canvasOn)
        {
            canvas.enabled = canvasOn;
        }
    }

    public Coroutine FadeScreenOverTime(float fadeTime, float fadeStart, float fadeEnd, Color fadeColor, bool allowPause = true)
    {
        return StartCoroutine(FadeScreenCr(fadeTime, fadeStart, fadeEnd, fadeColor, allowPause));
    }

    private IEnumerator FadeScreenCr(float fadeTime, float fadeStart, float fadeEnd, Color fadeColor, bool allowPause)
    {
        IsFadingScreen = true;
        float time = 0f;
        while (time < fadeTime)
        {
            if (allowPause)
            {
                yield return new WaitWhile(() => PH.Pause);
            }
            FadeScreen(Mathf.Lerp(fadeStart, fadeEnd, time / fadeTime), fadeColor);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        FadeScreen(fadeEnd, fadeColor);
        IsFadingScreen = false;
    }
}
