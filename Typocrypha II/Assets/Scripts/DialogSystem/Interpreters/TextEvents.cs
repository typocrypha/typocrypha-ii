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
            {"pause-dialog", PauseDialog},
            {"pause",PauseDialog },
            {"screen-shake",ScreenShake },
            {"shake", ScreenShake},
            {"text-delay", TextDelay},
            {"float-text", FloatText },
            {"tips-entry",SignalEntry },
            {"play-sfx",PlaySFX },
            {"next",NextDialog }
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
        Debug.Log("TextEvent:" + evt);
        if (!textEventMap.TryGetValue(evt, out var textEvent))
        {
            Debug.LogException(new System.Exception("Bad text event parameters:" + evt));
        }
        return textEvent(opt, box);
    }

    /**************************** TEXT EVENTS *****************************/

    /// <summary>
    /// Immediately go to next dialog box.
    /// </summary>
    /// <param name="opt">NONE</param>
    Coroutine NextDialog(string[] opt, DialogBox box)
    {
        DialogManager.instance.NextDialog();
        return null;
    }

    /// <summary>
    /// Test text event.
    /// </summary>
    /// <param name="opt">NONE</param>
    Coroutine Test(string[] opt, DialogBox box)
    {
        Debug.Log("test");
        return null;
    }

    Coroutine PlaySFX(string[] opt, DialogBox box)
    {
        //AudioManager.instance.PlaySFX(null);
        Debug.LogWarning("playSFX text event unsupported");
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
        DialogManager.instance.dialogBox.Pause();
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
        DialogManager.instance.dialogBox.Unpause();
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
        return StartCoroutine(ScreenShakeCR(opt, box));
    }

    private IEnumerator ScreenShakeCR(string[] opt, DialogBox box)
    {
        yield return CameraManager.instance.Shake(float.Parse(opt[0]), float.Parse(opt[1]));
        yield return new WaitUntil(() => box.IsDone);
        CameraManager.instance.ResetCamera();
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

    /// <summary>
    /// Spawns a single line of floating text.
    /// </summary>
    /// <param name="opt">
    /// [0]: string, line to display.
    /// [1-2]: float, x-y coordinate position.
    /// </param>
    Coroutine FloatText(string[] opt, DialogBox box)
    {
        Vector2 pos = new Vector2(float.Parse(opt[1]), float.Parse(opt[2]));
        FloatDialog.instance.SpawnFloatDialog(opt[0], pos);
        return null;
    }

    /// <summary>
    /// Signal TIPS that a new entry has been discovered.
    /// </summary>
    /// <param name="opt">
    /// </param>
    Coroutine SignalEntry(string[] opt, DialogBox box)
    {
        TIPSManager.instance.SignalEntry(true);
        return null;
    }
}

