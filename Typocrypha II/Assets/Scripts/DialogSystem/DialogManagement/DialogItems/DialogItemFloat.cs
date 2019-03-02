using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog line data for floating text.
/// </summary>
public class DialogItemFloat : DialogItem
{
    public DialogItemFloat(string text) : base(text, null) { }

    public override Type GetView()
    {
        return null;
    }
}
