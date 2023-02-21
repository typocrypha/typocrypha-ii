using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Audio novel style dialog. i.e. Floating text over full background.
/// </summary>
public class DialogViewAN : DialogView
{
    private const int maxMessages = 10;
    [SerializeField] private RectTransform ANContent; // Content of scroll view
    [SerializeField] private Image background;
    [SerializeField] private DialogContinueIndicator continueIndicator;

    private readonly List<DialogBox> dialogBoxPool = new List<DialogBox>(maxMessages);
    private readonly List<DialogBox> activeDialogBoxes = new List<DialogBox>(maxMessages);

    private void Awake()
    {
        background.color = Color.clear;    
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemAN dialogItem))
            return null;
        if(activeDialogBoxes.Count >= maxMessages)
        {
            ClearLog();
        }
        DialogBox dialogBox;
        if(dialogBoxPool.Count > 0)
        {
            dialogBox = dialogBoxPool[dialogBoxPool.Count - 1];
            dialogBoxPool.RemoveAt(dialogBoxPool.Count - 1);
            dialogBox.transform.SetAsLastSibling();
            dialogBox.gameObject.SetActive(true);
        }
        else
        {
            dialogBox = Instantiate(dialogBoxPrefab, ANContent).GetComponent<DialogBox>();
        }
        activeDialogBoxes.Add(dialogBox);
        continueIndicator.SetDialogBox(dialogBox);
        dialogBox.dialogText.alignment = dialogItem.AlignmentOptions;
        dialogBox.SetupAndStartDialogBox(dialogItem);
        continueIndicator.Activate();
        return dialogBox;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
        if (!e)
        {
            background.color = Color.clear;
        }
    }

    public override IEnumerator PlayEnterAnimation()
    {
        var fadeIn = background.DOFade(1, 2);
        yield return fadeIn.WaitForCompletion();
    }

    public override IEnumerator PlayExitAnimation(bool isEndOfDialog)
    {
        ClearLog();
        var fadeOut = background.DOFade(0, 2);
        yield return fadeOut.WaitForCompletion();
    }

    // Clear all AN dialogue (TODO)
    public void ClearLog()
    {
        foreach(var dialogBox in activeDialogBoxes)
        {
            dialogBox.gameObject.SetActive(false);
            dialogBoxPool.Add(dialogBox);
        }
        activeDialogBoxes.Clear();
    }

    public override void CleanUp()
    {
        ClearLog();
    }
}
