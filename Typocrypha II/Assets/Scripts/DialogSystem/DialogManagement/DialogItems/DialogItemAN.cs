using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog line data for audio novel style dialog.
/// </summary>
public class DialogItemAN : DialogItem
{
    public DialogItemAN(string text, List<AudioClip> voice) : base(text, voice) { }

    public override System.Type GetView()
    {
        return typeof(DialogViewAN);
    }
}
