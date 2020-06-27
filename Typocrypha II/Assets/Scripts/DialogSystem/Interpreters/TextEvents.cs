using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerator TextEventDel(string[] opt);

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
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
    }
    #endregion

    public static TextEvents instance = null;
    // Map of commands to text event handles.
    public Dictionary<string, TextEventDel> textEventMap;

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

        ph = new PauseHandle(OnPause);
        textEventMap = new Dictionary<string, TextEventDel>
        {
            {"test", Test },
            {"pause-dialog", PauseDialog},
            {"shake", ScreenShake},
            {"fade-screen", FadeScreen},
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
    public Coroutine PlayEvent(string evt, string[] opt)
    { 
        Debug.Log("TextEvent:" + evt);
        if (!textEventMap.TryGetValue(evt, out TextEventDel textEvent))
        {
            Debug.LogException(new System.Exception("Bad text event parameters:" + evt));
        }
        Coroutine cr = StartCoroutine(textEvent(opt));
        return cr;
    }

    /**************************** TEXT EVENTS *****************************/

    /// <summary>
    /// Immediately go to next dialog box.
    /// </summary>
    /// <param name="opt">NONE</param>
    IEnumerator NextDialog(string[] opt)
    {
        yield return null;
        DialogManager.instance.NextDialog();
    }

    /// <summary>
    /// Test text event.
    /// </summary>
    /// <param name="opt">NONE</param>
    IEnumerator Test(string[] opt)
    {
        Debug.Log("test");
        yield return null;
    }

    IEnumerator PlaySFX(string[] opt)
    {
        //AudioManager.instance.PlaySFX(null);
        yield return null;
    }

    /// <summary>
    /// Pauses text scroll for a fixed amount of time.
    /// Automatically unpauses if text scroll is done.
    /// </summary>
    /// <param name="opt">
    /// [0]:float: Length of pause.
    /// </param>
    IEnumerator PauseDialog(string[] opt)
    {
        DialogManager.instance.dialogBox.PH.Pause = true;
        float time = 0f;
        float endTime = float.Parse(opt[0]);
        while (time < endTime && !DialogManager.instance.dialogBox.IsDone)
        {
            yield return new WaitWhile(() => PH.Pause);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        DialogManager.instance.dialogBox.PH.Pause = false;
    }

    /// <summary>
    /// Shakes the screen.
    /// </summary>
    /// <param name="opt">
    /// [0]: float, intensity of shake.
    /// [1]: float, length of shake.
    /// </param>
    IEnumerator ScreenShake(string[] opt)
    {
        Coroutine shake = CameraManager.instance.Shake(float.Parse(opt[0]), float.Parse(opt[1]));
        yield return new WaitUntil(() => DialogManager.instance.dialogBox.IsDone);
        CameraManager.instance.ResetCamera();
    }

    /// <summary>
    /// Fades the screen.
    /// Automatically finishes fade when dialog line finishes.
    /// </summary>
    /// <param name="opt">
    /// [0]: float, amount of time to get to fade.
    /// [1]: float, starting amount of fade (0 to 1) (0: No fade).
    /// [2]: float, ending amount of fade.
    /// [3-5]: floats, rgb values (normalized) of color.
    /// </param>
    IEnumerator FadeScreen(string[] opt)
    {
        float time = 0f;
        float endTime = float.Parse(opt[0]);
        float init = float.Parse(opt[1]);
        float target = float.Parse(opt[2]);
        Color color = new Color(float.Parse(opt[3]), float.Parse(opt[4]), float.Parse(opt[5]), 1f);
        while (time < endTime && !DialogManager.instance.dialogBox.IsDone)
        {
            yield return new WaitWhile(() => PH.Pause);
            FaderManager.instance.FadeAll(Mathf.Lerp(init, target, time / endTime), color);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        FaderManager.instance.FadeAll(target, color);
    }

    /// <summary>
    /// Sets the text scroll speed. Only lasts for current dialog box.
    /// </summary>
    /// <param name="opt">
    /// [0]: float, amount of seconds delay between character reveals.
    /// </param>
    IEnumerator TextDelay(string[] opt)
    {
        DialogManager.instance.dialogBox.ScrollDelay = float.Parse(opt[0]);
        yield return null;
    }

    /// <summary>
    /// Spawns a single line of floating text.
    /// </summary>
    /// <param name="opt">
    /// [0]: string, line to display.
    /// [1-2]: float, x-y coordinate position.
    /// </param>
    IEnumerator FloatText(string[] opt)
    {
        Vector2 pos = new Vector2(float.Parse(opt[1]), float.Parse(opt[2]));
        FloatDialog.instance.SpawnFloatDialog(opt[0], pos);
        yield return null;
    }

    /// <summary>
    /// Signal TIPS that a new entry has been discovered.
    /// </summary>
    /// <param name="opt">
    /// </param>
    IEnumerator SignalEntry(string[] opt)
    {
        TIPSManager.instance.SignalEntry(true);
        yield return null;
    }
}

