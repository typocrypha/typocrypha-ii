using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for multiple dialog boxes scrolling at once.
/// </summary>
public class DialogBox_Multi : DialogBox, IDialogBox
{
    public List<DialogBox> boxes = new List<DialogBox>();

    #region IPausable
    /// <summary>
    /// Pauses text scroll of all boxes.
    /// </summary>
    PauseHandle ph;
    public new PauseHandle PH { get => ph; }

    public new void OnPause(bool b)
    {
        foreach (var box in boxes) box.PH.Pause = b;
    }
    #endregion

    public new float ScrollDelay
    {
        get => boxes[0].ScrollDelay * PlayerDataManager.instance.Get<float>(PlayerDataManager.textDelayScale);
        set => boxes[0].ScrollDelay = value;
    }

    /// <summary>
    /// Returns whether text is done scrolling or not for all boxes.
    /// </summary>
    public new bool IsDone
    {
        get
        {
            bool done = true;
            foreach (var box in boxes) done &= box.IsDone;
            return done;
        }
    }

    void Awake()
    {
        ph = new PauseHandle(OnPause);
    }

    /// <summary>
    /// Dumps all remaining text for all boxes.
    /// </summary>
    public new void DumpText()
    {
        foreach (var box in boxes) box.DumpText();
    }
}
