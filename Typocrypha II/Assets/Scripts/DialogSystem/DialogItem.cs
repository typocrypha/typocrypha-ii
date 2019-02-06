using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parsed line of dialog. 
/// </summary>
public class DialogItem : ScriptableObject
{
    public string text; // Text of dialog
    public List<FXText.FXTextBase> FXTextList; // List of all FXText effects
    public List<TextEvent> TextEventList; // List of all Text events
}
