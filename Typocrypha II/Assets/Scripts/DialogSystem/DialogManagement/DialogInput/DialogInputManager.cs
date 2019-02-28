using System.Collections;
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
        
        enabled = false;
    }

    /// <summary>
    /// Enables current line of dialog for input.
    /// Input prompt is displayed after text is fully displayed.
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
        DialogManager.instance.PH.Pause = true; // Disable player skipping dialog.
        DialogManager.instance.dialogView.DisplayInput(data); // Display dialog.
    }

    /// <summary>
    /// Called when input is submitted.
    /// </summary>
    /// <param name="value">Submitted value.</param>
    public void SubmitInput(string value)
    {
        PlayerDataManager.instance.tmpData[target] = value; // Set input value.
        DialogManager.instance.NextDialog(); // Start next dialog.
        DialogManager.instance.PH.Pause = false; // Re-enable dialog skipping.
    }
}
