﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player input during dialog.
/// Enabled when dialog line requires input. Disabled otherwise.
/// </summary>
public class DialogInputManager : MonoBehaviour
{
    public static DialogInputManager instance = null;
    public static Dictionary<string, string> tmpData; // Temporary data storage.

    string target;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        tmpData = new Dictionary<string, string>();
        enabled = false;
    }

    /// <summary>
    /// Enables current line of dialog for input.
    /// Input prompt is displayed after text scroll.
    /// </summary>
    /// <param name="data">Information about input event.</param>
    public void EnableInput(DialogInputItem data)
    {
        target = data.variableName; // Set input target.
        StartCoroutine(WaitForText(data));
    }

    // Wait for end of text scroll to display dialog input display.
    IEnumerator WaitForText(DialogInputItem data)
    {
        yield return new WaitWhile(() => DialogManager.instance.dialogBox.IsDone); // COULD HAVE SYNCHRO ISSUES
        yield return new WaitUntil(() => DialogManager.instance.dialogBox.IsDone); // THAT IS, SPEEDRUN TECH
        DialogManager.instance.enabled = false; // Disable player skipping dialog.
        DialogManager.instance.dialogView.DisplayInput(data); // Display dialog.
    }

    /// <summary>
    /// Called when input is submitted.
    /// </summary>
    /// <param name="value">Submitted value.</param>
    public void SubmitInput(string value)
    {
        tmpData[target] = value; // Set input value.
        DialogManager.instance.NextDialog(); // Start next dialog.
        DialogManager.instance.enabled = true; // Re-enable dialog skipping.
    }
}
