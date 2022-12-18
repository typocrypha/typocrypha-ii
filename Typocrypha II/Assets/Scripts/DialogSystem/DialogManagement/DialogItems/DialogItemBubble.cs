using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog line data for text bubble style dialog.
/// </summary>
public class DialogItemBubble : DialogItem
{
    public List<CharacterData> CharacterData { get; }
    public Vector2Int GridPosition { get; set; }
    public Vector2 AbsolutePosition { get; set; }

    public DialogItemBubble(string text, List<AudioClip> voice, List<CharacterData> characterData, Vector2Int gridPosition, Vector2 absolutePosition) : base(text, voice)
    {
        CharacterData = characterData;
        GridPosition = gridPosition;
        AbsolutePosition = absolutePosition;
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