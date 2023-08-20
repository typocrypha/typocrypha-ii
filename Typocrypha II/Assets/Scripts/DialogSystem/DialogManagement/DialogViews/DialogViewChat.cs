using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IconSide { LEFT, RIGHT, NONE }; // Side which icon displays

/// <summary>
/// Chat style dialog. i.e. chatboxes on scrollable feed.
/// </summary>
public class DialogViewChat : DialogViewMessage<DialogItemChat>
{
    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private GameObject narratorDialogBoxPrefab;
    [SerializeField] private GameObject randoDialogBoxPrefab;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private TweenInfo enterExitViewTween;

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemChat dialogItem))
            return null;
        return CreateNewMessage(dialogItem);
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }

    protected override GameObject GetMessagePrefab(DialogItemChat dialogItem, List<CharacterData> data, out bool isNarrator)
    {
        isNarrator = false;
        if (data.Count > 1)
        {
            throw new System.NotImplementedException("Chat mode doesn't currently support multi-character dialog lines");
        }
        if (data.Count <= 0)
        {
            throw new System.NotImplementedException("Chat mode doesn't currently support dialog lines with no character data");
        }
        var chara = data[0];
        if (chara == null)
        {
            return randoDialogBoxPrefab;
        }
        if(dialogItem.iconSide == IconSide.LEFT)
        {
            return leftDialogBoxPrefab;
        }
        else if (dialogItem.iconSide == IconSide.RIGHT)
        {
            return rightDialogBoxPrefab;
        }
        isNarrator = true;
        return narratorDialogBoxPrefab;
    }

    public override IEnumerator PlayEnterAnimation()
    {
        contentRoot.localScale = new Vector3(contentRoot.localScale.x, 0, contentRoot.localScale.z);
        enterExitViewTween.Start(contentRoot.DOScaleY(1, enterExitViewTween.Time));
        yield return enterExitViewTween.WaitForCompletion();
    }
    public override IEnumerator PlayExitAnimation(DialogManager.EndType endType)
    {
        contentRoot.localScale = new Vector3(contentRoot.localScale.x, 1, contentRoot.localScale.z);
        enterExitViewTween.Complete();
        enterExitViewTween.Start(contentRoot.DOScaleY(0, enterExitViewTween.Time), false);
        yield return enterExitViewTween.WaitForCompletion();
    }
}
