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
    [SerializeField] private RectTransform ANContent; // Content of scroll view
    [SerializeField] private Image background;

    private void Awake()
    {
        background.color = Color.clear;    
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemAN dialogItem))
            return null;       
        var dialogBox = Instantiate(dialogBoxPrefab, ANContent).GetComponent<DialogBox>();
        dialogBox.SetupAndStartDialogBox(dialogItem);
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
        var fadeOut = background.DOFade(0, 2);
        yield return fadeOut.WaitForCompletion();
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
