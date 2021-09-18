using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio novel style dialog. i.e. Floating text over full background.
/// </summary>
public class DialogViewAN : DialogView
{
    public RectTransform ANContent; // Content of scroll view

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemAN dialogItem))
            return null;
        
        var dialogBox = Instantiate(dialogBoxPrefab, ANContent).GetComponent<DialogBox>();
        dialogBox.StartDialogBox(dialogItem);
        return dialogBox;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }

    // Clear all AN dialogue (TODO)
    public void ClearLog()
    {
        
    }

    public override void CleanUp()
    {
        ClearLog();
    }
}
