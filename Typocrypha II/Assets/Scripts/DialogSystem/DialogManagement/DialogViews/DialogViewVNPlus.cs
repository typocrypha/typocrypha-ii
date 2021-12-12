using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogViewVNPlus : DialogView
{
    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private RectTransform messageContainer;
    [SerializeField] private VerticalLayoutGroup messageLayout;
    [SerializeField] private Ease messageLayoutEase;
    [SerializeField] private bool useCustomMessageLayoutEase;
    [SerializeField] private AnimationCurve customMessageLayoutEase;

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
        StartCoroutine(AnimateNewMessageIn(dialogBox, dialogItem));
        return dialogBox;
    }

    private IEnumerator AnimateNewMessageIn(DialogBox box, DialogItem item)
    {
        box.SetupDialogBox(item);
        var tween = messageContainer.DOAnchorPosY(messageContainer.anchoredPosition.y + (box.GetBoxHeight() + messageLayout.spacing), 0.5f);
        // Play animation
        if (useCustomMessageLayoutEase)
        {
            tween.SetEase(customMessageLayoutEase);
        }
        else
        {
            tween.SetEase(messageLayoutEase);
        }
        yield return new WaitForSeconds(1);
        box.StartDialogScroll();
        yield break;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
