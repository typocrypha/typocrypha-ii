using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog line data for audio novel style dialog.
/// </summary>
public class DialogItemLocation : DialogItem
{
    public DialogItemLocation(string text, List<AudioClip> voice) : base(text, voice) { }
    public override Type GetView()
    {
        return typeof(DialogViewLocation);
    }
}
