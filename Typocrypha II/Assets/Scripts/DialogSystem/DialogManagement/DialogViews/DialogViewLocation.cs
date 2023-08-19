using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// View to do establishing shots for locations
/// </summary>
public class DialogViewLocation : DialogView
{
    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private Image background;
    [SerializeField] private CanvasGroup textGroup;
    [SerializeField] private DialogContinueIndicator continueIndicator;
    [SerializeField] private float targetBgAlpha = 1;

    private void Awake()
    {
        background.color = Color.clear;
        dialogBox.ContinueIndicator = continueIndicator;
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemLocation dialogItem))
            return null;
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
        textGroup.alpha = 1;
        var fadeIn = background.DOFade(targetBgAlpha, 1);
        yield return fadeIn.WaitForCompletion();
    }

    public override IEnumerator PlayExitAnimation(DialogManager.EndType endType)
    {
        yield return textGroup.DOFade(0, 0.5f).WaitForCompletion();
        yield return background.DOFade(0, 0.5f).WaitForCompletion();
    }

    public override void CleanUp() { }
}
