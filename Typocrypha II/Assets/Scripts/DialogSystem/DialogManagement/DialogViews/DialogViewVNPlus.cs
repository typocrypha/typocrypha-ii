using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogViewVNPlus : DialogView
{
    private const float tweenTime = 0.5f;

    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private RectTransform messageContainer;
    [SerializeField] private VerticalLayoutGroup messageLayout;
    [SerializeField] private Ease messageLayoutEase;
    [SerializeField] private bool useCustomMessageLayoutEase;
    [SerializeField] private AnimationCurve customMessageLayoutEase;

    public override bool ReadyToContinue => readyToContinue;

    private bool readyToContinue = false;
    private Tween tween;

    // Do Tween Of Some sort for the animation
    public override void CleanUp()
    {

    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemVNPlus dialogItem))
            return null;
        var prefab = dialogItem.IsLeft ? leftDialogBoxPrefab : rightDialogBoxPrefab;
        var dialogBox = Instantiate(prefab, messageContainer).GetComponent<DialogBox>();
        readyToContinue = false;
        StartCoroutine(AnimateNewMessageIn(dialogBox, dialogItem));
        return dialogBox;
    }

    private IEnumerator AnimateNewMessageIn(DialogBox box, DialogItem item)
    {
        box.SetupDialogBox(item);
        yield return null;
        box.SetBoxHeight();
        tween?.Complete();
        tween = messageContainer.DOAnchorPosY(messageContainer.anchoredPosition.y + (box.GetBoxHeight() + messageLayout.spacing), tweenTime);
        // Play animation
        if (useCustomMessageLayoutEase)
        {
            tween.SetEase(customMessageLayoutEase);
        }
        else
        {
            tween.SetEase(messageLayoutEase);
        }
        readyToContinue = true;
        box.StartDialogScroll();
        yield break;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
