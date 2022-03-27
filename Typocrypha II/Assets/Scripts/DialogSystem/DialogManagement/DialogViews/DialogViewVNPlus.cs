using System.Collections;
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

    public override void CleanUp()
    {

    }

    public bool IsReadyToContinue() => readyToContinue;

    #region Character Control

    public void AddCharacter(CharacterData data, CharacterColumn column)
    {
        if(!characterMap.ContainsKey(data.name))// Scene character
        {
            readyToContinue = false;
            StartCoroutine(AddCharacterCR(data, column));
        }
    }
    private IEnumerator AddCharacterCR(CharacterData data, CharacterColumn column)
    {
        VNPlusCharacter newCharacter;
        if (column == CharacterColumn.Right)
        {
            if(rightCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListCR(leftCharacterContainer, rightCharacterList));
            }
            newCharacter = InstantiateCharacter(rightCharacterPrefab, rightCharacterContainer, rightCharacterList);
        }
        else
        {
            if (leftCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListCR(leftCharacterContainer, leftCharacterList));
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

    private IEnumerator AdjustCharacterListCR(RectTransform container, List<VNPlusCharacter> characterList)
    {
        float newHeight = container.rect.height / (characterList.Count + 1);
        float posStart = -newHeight / 2;
        float staggerTime = 0;
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            chara.DoAdjustHeightTween(newHeight);
            staggerTime = Mathf.Max(chara.AdjustTweenTime / 2, staggerTime);
        }
        yield return new WaitForSeconds(staggerTime);
        Tween tween = null;
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            tween = chara.DoAdjustPosTween(posStart - (i * newHeight));
        }
        yield return tween.WaitForCompletion();
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
