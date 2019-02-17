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
public class TextEvents : MonoBehaviour
{
	public static TextEvents instance = null;
	public Dictionary<string, TextEventDel> textEventMap; // Map of commands to text event handles.

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
        
        textEventMap = new Dictionary<string, TextEventDel>
        {
            {"pause", Pause},
            {"shake", ScreenShake},
            {"fade-screen", FadeScreen}
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
        if (!textEventMap.TryGetValue(evt, out TextEventDel textEvent))
        {
            Debug.LogException(new System.Exception("Bad text event parameters:" + evt));
        }
        Coroutine cr = StartCoroutine(textEvent(opt));
        return cr;
    }

    /**************************** TEXT EVENTS *****************************/

    /// <summary>
    /// Pauses text scroll for a fixed amount of time.
    /// Automatically unpauses if text scroll is done.
    /// </summary>
    /// <param name="opt">
    /// [0]:float: Length of pause.
    /// </param>
    IEnumerator Pause(string[] opt)
    {
        DialogManager.instance.dialogBox.Pause = true;
        float time = 0f;
        float endTime = float.Parse(opt[0]);
        while (time < endTime && !DialogManager.instance.dialogBox.IsDone)
        {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        DialogManager.instance.dialogBox.Pause = false;
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
            FaderManager.instance.FadeAll(Mathf.Lerp(init, target, time / endTime), color);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        FaderManager.instance.FadeAll(target, color);
    }
}

