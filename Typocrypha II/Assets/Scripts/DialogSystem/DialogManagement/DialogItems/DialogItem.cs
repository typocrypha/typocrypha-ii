using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single line of dialogue, and all it's effects/events/etc
/// </summary>
public abstract class DialogItem
{
    public string text; // Text of dialog.
    public List<AudioClip> voice; // Voice blip audio clips.
    public List<MonoBehaviour> FXTextList { get; } = new List<MonoBehaviour>(); // List of all FXText effects.
    public List<TextEvent> TextEventList { get; } = new List<TextEvent>(); // List of all Text events.
    public DialogItem(string text, List<AudioClip> voice)
    {
        this.text = text;
        this.voice = voice;
    }

    /// <summary>
    /// Get associated dialog view type. Should be overriden.
    /// </summary>
    /// <returns>Type of dialog view.</returns>
    public abstract System.Type GetView();

    public virtual void Clear()
    {
        FXTextList.Clear();
        TextEventList.Clear();
    }
}