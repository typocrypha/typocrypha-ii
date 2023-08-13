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
    [SerializeField] private List<AudioClip> scrollSfx;
    [SerializeField] private float targetBgAlpha = 1;

    public override bool DeactivateOnEndSceneHide => false;

    private void Awake()
    {
        background.color = Color.clear;    
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        dialogBox.SetupAndStartDialogBox(data);
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
        PlayDialog(new DialogItemLocation(DialogManager.instance.LocationText, scrollSfx));
        yield return new WaitUntil(DialogBoxDone);
        continueIndicator.Activate();
        yield return new WaitUntil(GotoNext);
    }

    private bool DialogBoxDone() => dialogBox.IsDone;
    private bool GotoNext() => Input.GetKeyDown(KeyCode.Space);

    public override IEnumerator PlayExitAnimation(DialogManager.EndType endType)
    {
        yield return textGroup.DOFade(0, 0.5f).WaitForCompletion();
        yield return background.DOFade(0, 0.5f).WaitForCompletion();
    }

    public override void CleanUp() { }
}
