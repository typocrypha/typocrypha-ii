using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog line data for audio novel style dialog.
/// </summary>
public class DialogItemAN : DialogItem
{
    public TMPro.TextAlignmentOptions AlignmentOptions { get; }
    public TextAnchor LayoutSetting { get; }

    public DialogItemAN(string text, List<AudioClip> voice, TMPro.TextAlignmentOptions textAlignment = TMPro.TextAlignmentOptions.Left, TextAnchor layout = TextAnchor.UpperLeft) : base(text, voice) 
    {
        AlignmentOptions = textAlignment;
        LayoutSetting = layout;
    }

    public override System.Type GetView()
    {
        return typeof(DialogViewAN);
    }
}
