using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IconSide {LEFT, RIGHT, BOTH, NONE}; // Side which icon displays
public enum DialogueType {NORMAL, INPUT}; // Type of player interaction with dialouge

/// <summary>
/// Represents a single line of dialogue, and all it's effects/events/etc
/// </summary>
public class DialogItem
{
    public string text; // Text of dialog
    public List<FXText.FXTextBase> FXTextList; // List of all FXText effects
    public List<TextEvent> TextEventList; // List of all Text events
    public DialogItem(string text)
    {
        this.text = text;
    }
}




