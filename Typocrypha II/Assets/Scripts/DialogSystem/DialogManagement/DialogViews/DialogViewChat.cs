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
    private const int maxCharacters = 7;
    [Header("Dialog Box Prefabs")]
    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private GameObject narratorDialogBoxPrefab;
    [SerializeField] private GameObject randoDialogBoxPrefab;
    [SerializeField] private GameObject embeddedImagePrefab;
    [Header("Enter / Exit Anim Parameters")]
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private TweenInfo enterExitViewTween;
    [Header("Character Panel")]
    [SerializeField] private List<ChatCharacter> onlineCharacterList;
    [SerializeField] private List<ChatCharacter> offlineCharacterList;
    private int onlineCharacters = 0;
    private int offlineCharacters = 0;

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemChat dialogItem))
            return null;
        var dialogBox = CreateNewMessage(dialogItem);
        var chatUI = dialogBox.GetComponent<ChatDialogBoxUI>();
        if(chatUI != null)
        {
            chatUI.Bind(dialogItem);
        }
        return dialogBox;
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

    protected override GameObject GetImagePrefab()
    {
        return embeddedImagePrefab;
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

    #region Character Control

    public override bool AddCharacter(AddCharacterArgs args)
    {
        if (onlineCharacters + offlineCharacters >= maxCharacters)
        {
            Debug.LogError("Too many chat characters, character will not be added");
            return false;
        }
        SetCharacterOnlineStatus(args.CharacterData, args.Column == CharacterColumn.Right);
        return false;
    }

    public override bool RemoveCharacter(CharacterData data)
    {
        SetCharacterOnlineStatus(data, false);
        return false;
    }

    private void SetCharacterOnlineStatus(CharacterData data, bool online)
    {
        if (online)
        {
            SetCharacter(data, onlineCharacterList, ref onlineCharacters, offlineCharacterList, ref offlineCharacters);
        }
        else
        {
            SetCharacter(data, offlineCharacterList, ref offlineCharacters, onlineCharacterList, ref onlineCharacters);
        }
    }

    private void SetCharacter(CharacterData data, List<ChatCharacter> list, ref int num, List<ChatCharacter> otherList, ref int otherNum)
    {
        if (FindCharacter(data, list, num, out int _))
        {
            return;
        }
        if (FindCharacter(data, otherList, otherNum, out int characterIndex))
        {
            otherList[characterIndex].gameObject.SetActive(false);
            otherNum--;
            otherList.Sort();
        }
        var character = list[num++];
        character.gameObject.SetActive(true);
        character.Data = data;
    }

    private bool FindCharacter(CharacterData data, List<ChatCharacter> list, int num, out int characterIndex)
    {
        for (int i = 0; i < num; i++)
        {
            if (list[i].Data == data)
            {
                characterIndex = i;
                return true;
            }
        }
        characterIndex = -1;
        return false;
    }

    #endregion

    public override void CleanUp()
    {
        base.CleanUp();
        foreach(var character in onlineCharacterList)
        {
            character.gameObject.SetActive(false);
        }
        onlineCharacters = 0;
        foreach (var character in offlineCharacterList)
        {
            character.gameObject.SetActive(false);
        }
        offlineCharacters = 0;
    }
}
