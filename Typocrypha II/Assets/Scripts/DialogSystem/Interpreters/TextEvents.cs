using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct to contain a text event.
/// </summary>
public struct TextEvent
{ 
    public string evt; // Name of event
    public string[] opt; // Options of event
    public int pos; // Position in text of event
    public TextEvent(string evt, string[] opt, int pos)
    {
        this.evt = evt;
        this.opt = opt;
        this.pos = pos;
    }
}

/// <summary>
/// Event class for events during text dialogue.
/// Individual events are Coroutine handles.
/// Events should handle by themselves the case where dialog is skipped mid scroll.
/// </summary>
public class TextEvents : MonoBehaviour, IPausable
{
    public const string pauseEvent = "pause";
    #region IPausable
    public PauseHandle PH { get; private set; }

    public void OnPause(bool b)
    {
    }
    #endregion

    public static TextEvents instance = null;
    // Map of commands to text event handles.
    public Dictionary<string, Func<string[], DialogBox, Coroutine>> textEventMap;

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

        PH = new PauseHandle(OnPause);
        textEventMap = new Dictionary<string, Func<string[], DialogBox, Coroutine>>
        {
            {"test", Test },
            {pauseEvent,PauseDialog },
            {"screen-shake",ScreenShake },
            {"shake", ScreenShake},
            {"text-delay", TextDelay},
            {"scroll-delay", TextDelay},
            {"speech-interval", SpeechInterval},
            {"play-sfx", PlaySFX },
            {"next", ContinueDialog },
        };
    }

    /// <summary>
    /// Plays a text event.
    /// </summary>
    /// <param name="evt">Name of event.</param>
    /// <param name="opt">Parameters to event.</param>
    /// <returns>Coroutine of event (null if none).</returns>
    public Coroutine PlayEvent(string evt, string[] opt, DialogBox box)
    { 
        if (!textEventMap.TryGetValue(evt, out var textEvent))
        {
            Debug.LogException(new System.Exception("Bad text event parameters:" + evt));
        }
        return textEvent(opt, box);
    }

    /**************************** TEXT EVENTS *****************************/

    /// <summary>
    /// Test text event.
    /// </summary>
    /// <param name="opt">NONE</param>
    Coroutine Test(string[] opt, DialogBox box)
    {
        Debug.Log("test");
        return null;
    }

    /// <summary>
    /// Play a sfx clip.
    /// </summary>
    /// <param name="opt">
    /// [0]:string: Name of sound clip.
    /// </param>
    Coroutine PlaySFX(string[] opt, DialogBox box)
    {
        AudioManager.instance.PlaySFX(opt[0]);
        return null;
    }

    /// <summary>
    /// Pauses text scroll for a fixed amount of time.
    /// Automatically unpauses if text scroll is done.
    /// </summary>
    /// <param name="opt">
    /// [0]:float: Length of pause.
    /// </param>
    Coroutine PauseDialog(string[] opt, DialogBox box)
    {
        return StartCoroutine(PauseDialogCR(opt, box));
    }

    private IEnumerator PauseDialogCR(string[] opt, DialogBox box)
    {
        float time = 0f;
        float endTime = float.Parse(opt[0]);
        while (time < endTime && !box.IsDone)
        {
            yield return new WaitWhile(this.IsPaused);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Shakes the screen.
    /// </summary>
    /// <param name="opt">
    /// [0]: float, intensity of shake.
    /// [1]: float, length of shake.
    /// </param>
    Coroutine ScreenShake(string[] opt, DialogBox box)
    {
        CameraManager.instance.Shake(float.Parse(opt[0]), float.Parse(opt[1]));
        if (DialogManager.instance.DialogView is DialogViewVNPlus)
        {
            var ppu = Screen.height / (CameraManager.instance.defaultCameraSize * 2);
            (DialogManager.instance.DialogView as DialogViewVNPlus).Shake(
                float.Parse(opt[0]) * ppu * 0.25f, //TODO: fix magic number
                float.Parse(opt[1]));
        }
        return null;
    }

    /// <summary>
    /// Sets the text scroll speed. Only lasts for current dialog box.
    /// </summary>
    /// <param name="opt">
    /// [0]: float, amount of seconds delay between character reveals.
    /// </param>
    Coroutine TextDelay(string[] opt, DialogBox box)
    {
        box.ScrollDelay = float.Parse(opt[0]);
        return null;
    }

    Coroutine SpeechInterval(string[] opt, DialogBox box)
    {
        box.SpeechInterval = int.Parse(opt[0]);
        if(opt.Length > 1)
        {
            box.PlaySpeechOnSpaces = bool.Parse(opt[1]);
        }
        return null;
    }

    Coroutine ContinueDialog(string[] opt, DialogBox box)
    {
        float delay = opt.Length > 0 ? float.Parse(opt[0]) : DialogBox.defaultDashContinueDelay;
        return StartCoroutine(ContinueDialogCR(delay));
    }

    IEnumerator ContinueDialogCR(float delay)
    {
        yield return new WaitForSeconds(delay);
        DialogManager.instance.NextDialog(true, false);
    }

}

