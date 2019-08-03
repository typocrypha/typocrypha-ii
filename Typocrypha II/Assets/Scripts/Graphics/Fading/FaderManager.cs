using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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

    public static FaderManager instance = null;
    public List<Fader> allFaders; // List of all faders. Fader instances add themselves.

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
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
            target.GetComponent<FaderGroup>().FadeAmount = 0f;
            target.GetComponent<FaderGroup>().FadeColor = Color.black;
        }
    }
}
