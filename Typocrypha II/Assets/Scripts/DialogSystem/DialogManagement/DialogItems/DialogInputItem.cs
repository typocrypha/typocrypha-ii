using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for information about input events.
/// </summary>
public class DialogInputItem
{
    public string variableName; // Name of variable to save to

    public DialogInputItem(string variableName)
    {
        this.variableName = variableName;
    }
}
