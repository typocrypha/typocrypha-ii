using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio novel style dialog. i.e. Floating text over full background.
/// </summary>
public class DialogViewAN : DialogView
{
    public GameObject screenFrame; // Screen frame object
    public RectTransform ANContent; // Content of scroll view
    public GameObject spacebar_icon_an; // Spacebar icon AN view
    public Animator animator_spacebar_an; // Spacebar icon key animator

    public override DialogBox PlayDialog(DialogItem data)
    {
        #region Check Arguments
        DialogItemAN dialogItem = data as DialogItemAN;
        if (dialogItem == null)
        {
            throw new System.Exception("Incorrect Type of dialog Item for the AN " +
                                       "view mode (requires DialogItemAN)");
        }
        #endregion
        
        DialogBox dialogBox = Instantiate(dialogBoxPrefab, ANContent).GetComponent<DialogBox>();
        dialogBox.StartDialogBox(dialogItem);
        return dialogBox;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
        screenFrame.SetActive(!e);
    }

    // Clear all AN dialogue
    public void ClearLog()
    {
        
    }
}
