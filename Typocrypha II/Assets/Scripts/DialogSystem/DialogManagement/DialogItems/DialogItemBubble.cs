using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog line data for text bubble style dialog.
/// </summary>
public class DialogItemBubble : DialogItem
{
    public Rect rect; // Position and size of speech bubble.

    public DialogItemBubble(string text, List<AudioClip> voice, Rect rect) : base(text, voice)
    {
        this.rect = rect;
    }

    public override Type GetView() => typeof(DialogViewBubble);
}

/// <summary>
/// Container for multiple speech bubble dialog items.
/// </summary>
public class DialogItemBubble_Multi : DialogItem
{
    public List<DialogItemBubble> bubbleList = new List<DialogItemBubble>();

    public DialogItemBubble_Multi(string text, List<AudioClip> voice) : base(text, voice)
    {
        
    }

    public override Type GetView() => typeof(DialogViewBubble);
}