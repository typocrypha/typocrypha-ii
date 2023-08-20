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
}
