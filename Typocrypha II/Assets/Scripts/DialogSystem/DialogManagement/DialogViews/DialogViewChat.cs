using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IconSide { LEFT, RIGHT, BOTH, NONE }; // Side which icon displays

/// <summary>
/// Chat style dialog. i.e. chatboxes on scrollable feed.
/// </summary>
public class DialogViewChat : DialogViewMessage
{

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemChat item))
            return null;

        //#region Instantiate and initialize new Dialog boxDialogueRightIcon
        //GameObject obj = GameObject.Instantiate(dialogBoxPrefab, ChatContent);
        //Image leftIcon = obj.transform.Find("DialogueLeftIcon").GetComponent<Image>();
        //Image rightIcon = obj.transform.Find("DialogueRightIcon").GetComponent<Image>();
        //if (item.iconSide == IconSide.LEFT || item.iconSide == IconSide.BOTH)
        //{
        //    leftIcon.sprite = item.leftIcon;
        //    leftIcon.enabled = true;
        //}
        //if (item.iconSide == IconSide.RIGHT || item.iconSide == IconSide.BOTH)
        //{
        //    rightIcon.sprite = item.rightIcon;
        //    rightIcon.enabled = true;
        //}
        //DialogBox dialogBox = obj.GetComponent<DialogBox>();
        //#endregion
        return null;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }

    protected override GameObject GetMessagePrefab(List<CharacterData> data, out bool isNarrator)
    {
        throw new System.NotImplementedException();
    }
}
