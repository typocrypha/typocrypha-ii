﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class DialogViewVNPlus : DialogView
{
    private const float tweenTime = 0.5f;
    private const int maxCharactersPerColumn = 5;

    public enum CharacterColumn
    {
        Right,
        Left,
    }

    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private GameObject narratorDialogBoxPrefab;
    [SerializeField] private RectTransform messageContainer;
    [SerializeField] private VerticalLayoutGroup messageLayout;
    [SerializeField] private Ease messageLayoutEase;
    [SerializeField] private bool useCustomMessageLayoutEase;
    [SerializeField] private AnimationCurve customMessageLayoutEase;
    [SerializeField] private GameObject rightCharacterPrefab;
    [SerializeField] private GameObject leftCharacterPrefab;
    [SerializeField] private RectTransform rightCharacterContainer;
    [SerializeField] private RectTransform leftCharacterContainer;


    public override bool ReadyToContinue => readyToContinue;

    private bool readyToContinue = false;
    private Tween messageTween;

    private readonly Dictionary<string, VNPlusCharacter> characterMap = new Dictionary<string, VNPlusCharacter>(maxCharactersPerColumn * 2);
    private readonly List<VNPlusCharacter> rightCharacterList = new List<VNPlusCharacter>(maxCharactersPerColumn);
    private readonly List<VNPlusCharacter> leftCharacterList = new List<VNPlusCharacter>(maxCharactersPerColumn);
    private float originalMessageAnchorPosY = float.MinValue;

    private void Awake()
    {
        originalMessageAnchorPosY = messageContainer.anchoredPosition.y;
    }

    public override void CleanUp()
    {
        ClearLog();
    }

    private void ClearLog()
    {
        StopAllCoroutines();
        foreach (Transform child in messageContainer)
        {
            Destroy(child.gameObject);
        }
        messageTween?.Complete();
        if (originalMessageAnchorPosY != float.MinValue)
        {
            messageContainer.anchoredPosition = new Vector2(messageContainer.anchoredPosition.x, originalMessageAnchorPosY);
        }
    }

    public bool IsReadyToContinue() => readyToContinue;

    #region Character Control

    public void RemoveCharacter(CharacterData data)
    {
        // If the character isn't in the scene, simply return
        if (!characterMap.ContainsKey(data.name))
        {
            return;
        }
        readyToContinue = false;
        StartCoroutine(RmCharacterCR(characterMap[data.name], data.name));
    }
    private IEnumerator RmCharacterCR(VNPlusCharacter character, string name)
    {

        yield return character.PlayLeaveTween().WaitForCompletion();
        if (character.Column == CharacterColumn.Right)
        {
            rightCharacterList.Remove(character);
            if (rightCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListPostLeaveCR(rightCharacterContainer, rightCharacterList));
            }
        }
        else
        {
            leftCharacterList.Remove(character);
            if (leftCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListPostLeaveCR(leftCharacterContainer, leftCharacterList));
            }
        }
        characterMap.Remove(name);
        readyToContinue = true;
    }

    public void AddCharacter(CharacterData data, CharacterColumn column)
    {
        // If the character is already in the scene, return
        if (characterMap.ContainsKey(data.name))
        {
            return;
        }
        readyToContinue = false;
        StartCoroutine(AddCharacterCR(data, column));
    }
    private IEnumerator AddCharacterCR(CharacterData data, CharacterColumn column)
    {
        VNPlusCharacter newCharacter;
        if (column == CharacterColumn.Right)
        {
            if(rightCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListPreJoinCR(rightCharacterContainer, rightCharacterList));
            }
            newCharacter = InstantiateCharacter(rightCharacterPrefab, rightCharacterContainer, rightCharacterList);
        }
        else
        {
            if (leftCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListPreJoinCR(leftCharacterContainer, leftCharacterList));
            }
            newCharacter = InstantiateCharacter(leftCharacterPrefab, leftCharacterContainer, leftCharacterList);
        }
        HighlightCharacter(data);
        newCharacter.Data = data;
        // TODO: name override
        newCharacter.NameText = data.mainAlias;
        characterMap.Add(data.name, newCharacter);
        yield return newCharacter.PlayJoinTween().WaitForCompletion();
        readyToContinue = true;
    }

    private VNPlusCharacter InstantiateCharacter(GameObject prefab, RectTransform container, List<VNPlusCharacter> characterList)
    {
        var newCharacter = Instantiate(prefab, container).GetComponent<VNPlusCharacter>();
        characterList.Add(newCharacter);
        var height = container.rect.height / characterList.Count;
        newCharacter.SetInitialHeight(height);
        newCharacter.SetInitialPos((-height / 2) - ((characterList.Count - 1) * height));

        return newCharacter;
    }

    private IEnumerator AdjustCharacterListPreJoinCR(RectTransform container, List<VNPlusCharacter> characterList)
    {
        GetCharacterAdjustmentValues(container, characterList, 1, out float newHeight, out float posStart);
        AdjustCharacterHeights(characterList, newHeight, out float staggerTime);
        yield return new WaitForSeconds(staggerTime);
        yield return AdjustCharacterPositions(characterList, posStart, newHeight, out float _).WaitForCompletion();
    }

    private IEnumerator AdjustCharacterListPostLeaveCR(RectTransform container, List<VNPlusCharacter> characterList)
    {
        GetCharacterAdjustmentValues(container, characterList, 0, out float newHeight, out float posStart);
        AdjustCharacterPositions(characterList, posStart, newHeight, out float staggerTime);
        yield return new WaitForSeconds(staggerTime);
        yield return AdjustCharacterHeights(characterList, newHeight, out float _).WaitForCompletion();
    }

    private void GetCharacterAdjustmentValues(RectTransform container, List<VNPlusCharacter> characterList, int extraCharacters, out float newHeight, out float posStart)
    {
        // Extra characters determines the space to leave for additional chracters that are not currently present
        // For example, use 1 extra character when a character is about to join to leave space for the joining character
        newHeight = container.rect.height / (characterList.Count + extraCharacters);
        posStart = -newHeight / 2;
    }

    private Tween AdjustCharacterHeights(List<VNPlusCharacter> characterList, float newHeight, out float staggerTime)
    {
        Tween tween = null;
        staggerTime = 0;
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            tween = chara.DoAdjustHeightTween(newHeight);
            staggerTime = Mathf.Max(chara.AdjustTweenTime / 2, staggerTime);
        }
        return tween;
    }

    private Tween AdjustCharacterPositions(List<VNPlusCharacter> characterList, float posStart, float newHeight, out float staggerTime)
    {
        Tween tween = null;
        staggerTime = 0;
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            tween = chara.DoAdjustPosTween(posStart - (i * newHeight));
            staggerTime = Mathf.Max(chara.AdjustTweenTime / 2, staggerTime);
        }
        return tween;
    }

    public void SetExpression(CharacterData data, string expression)
    {
        if (characterMap.ContainsKey(data.name))// Scene character
        {
            characterMap[data.name].SetExpression(expression);
        }
    }

    public void SetPose(CharacterData data, string pose)
    {
        if (characterMap.ContainsKey(data.name))// Scene character
        {
            characterMap[data.name].SetPose(pose);
        }
    }

    /// <summary>
    /// Turns highlight on or off on a character.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <param name="on">Whether to turn on highlight.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public void HighlightCharacter(IEnumerable<CharacterData> data)
    {
        foreach(var kvp in characterMap)
        {
            kvp.Value.Highlighted = data.Any(d => d.name == kvp.Key);
        }
    }

    public void HighlightCharacter(CharacterData data)
    {
        foreach (var kvp in characterMap)
        {
            kvp.Value.Highlighted = data.name == kvp.Key;
        }
    }

    #endregion

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemVNPlus dialogItem))
            return null;
        if (dialogItem.CharacterData.Count > 0 && characterMap.ContainsKey(dialogItem.CharacterData[0].name))
        {
            HighlightCharacter(dialogItem.CharacterData);
        }
        var prefab = GetMessagePrefab(dialogItem.CharacterData);
        var dialogBox = Instantiate(prefab, messageContainer).GetComponent<DialogBox>();
        SetCharacterSpecificUI(dialogBox, dialogItem.CharacterData);
        readyToContinue = false;
        StartCoroutine(AnimateNewMessageIn(dialogBox, dialogItem));
        return dialogBox;
    }

    private void SetCharacterSpecificUI(DialogBox messageBox, List<CharacterData> data)
    {
        var vnPlusUI = messageBox.GetComponent<VNPlusDialogBoxUI>();
        if (vnPlusUI == null)
            return;
        if (data.Count > 1)
        {
            throw new System.NotImplementedException("VNPlus mode doesn't currently support multi-character dialog lines");
        }
        if(data.Count <= 0)
        {
            throw new System.NotImplementedException("VNPlus mode doesn't currently support system dialog lines");
        }
        vnPlusUI.Bind(data[0]);
    }

    private GameObject GetMessagePrefab(List<CharacterData> data)
    {
        if(data.Count > 1)
        {
            throw new System.NotImplementedException("VNPlus mode doesn't currently support multi-character dialog lines");
        }
        if(data.Count <= 0)
        {
            throw new System.NotImplementedException("VNPlus mode doesn't currently support dialog lines with no character data");
        }
        var chara = data[0];
        if (characterMap.ContainsKey(chara.name))
        {
            return characterMap[chara.name].Column == CharacterColumn.Left ? leftDialogBoxPrefab : rightDialogBoxPrefab;
        }
        return narratorDialogBoxPrefab;
    }

    private IEnumerator AnimateNewMessageIn(DialogBox box, DialogItem item)
    {
        box.SetupDialogBox(item);
        yield return null;
        box.SetBoxHeight();
        messageTween?.Complete();
        messageTween = messageContainer.DOAnchorPosY(messageContainer.anchoredPosition.y + (box.GetBoxHeight() + messageLayout.spacing), tweenTime);
        // Play animation
        if (useCustomMessageLayoutEase)
        {
            messageTween.SetEase(customMessageLayoutEase);
        }
        else
        {
            messageTween.SetEase(messageLayoutEase);
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
