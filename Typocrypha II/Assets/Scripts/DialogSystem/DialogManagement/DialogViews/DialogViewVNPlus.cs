using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class DialogViewVNPlus : DialogView
{
    public const string narratorName = "narrator";
    private const int maxCharactersPerColumn = 5;
    private const int maxMessages = 7;
    private const int messagePrefabTypes = 3;
    private const float enterExitStaggerTime = 0.5f;
    private const float enterExitIndividualStaggerTime = 0.05f;

    public enum CharacterColumn
    {
        Right,
        Left,
    }

    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private GameObject narratorDialogBoxPrefab;
    [SerializeField] private GameObject randoDialogBoxPrefab;
    [SerializeField] private RectTransform messageContainer;
    [SerializeField] private VerticalLayoutGroup messageLayout;
    [SerializeField] private GameObject rightCharacterPrefab;
    [SerializeField] private GameObject leftCharacterPrefab;
    [SerializeField] private RectTransform rightCharacterContainer;
    [SerializeField] private RectTransform leftCharacterContainer;
    [SerializeField] private RectTransform contentRoot;

    [SerializeField] private TweenInfo messageTween;
    [SerializeField] private TweenInfo messageScaleTween;
    [SerializeField] private TweenInfo messageFadeTween;
    [SerializeField] private TweenInfo moveCharaToTopTween;
    [SerializeField] private TweenInfo characterJoinLeaveTween;
    [SerializeField] private TweenInfo enterExitViewTween;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI dateTimeText;


    public override bool ReadyToContinue => readyToContinue;
    private bool readyToContinue = true;

    private readonly Dictionary<string, VNPlusCharacter> characterMap = new Dictionary<string, VNPlusCharacter>(maxCharactersPerColumn * 2);
    private readonly List<VNPlusCharacter> rightCharacterList = new List<VNPlusCharacter>(maxCharactersPerColumn);
    private readonly List<VNPlusCharacter> leftCharacterList = new List<VNPlusCharacter>(maxCharactersPerColumn);
    private float originalMessageAnchorPosY = float.MinValue;
    private VNPlusDialogBoxUI lastBoxUI = null;

    private readonly List<DialogBox> dialogBoxPool = new List<DialogBox>(maxMessages * (messagePrefabTypes + 1));
    private readonly Queue<VNPlusCharacter> rightCharacterPool = new Queue<VNPlusCharacter>(maxCharactersPerColumn);
    private readonly Queue<VNPlusCharacter> leftCharacterPool = new Queue<VNPlusCharacter>(maxCharactersPerColumn);

    public override bool ShowImmediately => false;

    private void Awake()
    {
        originalMessageAnchorPosY = messageContainer.anchoredPosition.y;
    }

    public override void CleanUp()
    {
        StopAllCoroutines();
        ClearLog();
        // Add the character boxes back to the pool
        foreach (var chara in leftCharacterList)
        {
            chara.gameObject.SetActive(false);
            leftCharacterPool.Enqueue(chara);
        }
        foreach (var chara in rightCharacterList)
        {
            chara.gameObject.SetActive(false);
            rightCharacterPool.Enqueue(chara);
        }
        characterMap.Clear();
        rightCharacterList.Clear();
        leftCharacterList.Clear();
    }

    public override Coroutine Clear()
    {
        ClearLog();
        return null;
    }
    private void ClearLog()
    {
        StopAllCoroutines();
        CompleteMessageTweens();
        dialogBoxPool.Clear();
        foreach (Transform child in messageContainer)
        {
            Destroy(child.gameObject);
        }
        if (originalMessageAnchorPosY != float.MinValue)
        {
            messageContainer.anchoredPosition = new Vector2(messageContainer.anchoredPosition.x, originalMessageAnchorPosY);
        }
    }

    #region Character Control

    public override bool RemoveCharacter(CharacterData data)
    {
        // If the character isn't in the scene, simply return
        if (!characterMap.ContainsKey(data.name))
        {
            return false; // don't wait for completion
        }
        readyToContinue = false;
        StartCoroutine(RmCharacterCR(characterMap[data.name], data.name));
        return true; // Wait for completion
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
            leftCharacterPool.Enqueue(character);
            if (leftCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListPostLeaveCR(leftCharacterContainer, leftCharacterList));
            }
        }
        characterMap.Remove(name);
        readyToContinue = true;
    }

    public override bool AddCharacter(AddCharacterArgs args)
    {
        // If the character is already in the scene, return
        if (characterMap.ContainsKey(args.CharacterData.name))
        {
            return false;
        }
        if (isActiveAndEnabled)
        {
            readyToContinue = false;
            StartCoroutine(AddCharacterCR(args));
            return true;
        }
        AddCharacterInstant(args);
        return false;
    }
    private void AddCharacterInstant(AddCharacterArgs args)
    {
        VNPlusCharacter newCharacter;
        if (args.Column == CharacterColumn.Right)
        {
            if (rightCharacterList.Count > 0)
            {
                AdjustCharactersPreJoinInstant(rightCharacterContainer, rightCharacterList);
            }
            newCharacter = InstantiateCharacter(rightCharacterPrefab, rightCharacterContainer, rightCharacterList, rightCharacterPool);
        }
        else
        {
            if (leftCharacterList.Count > 0)
            {
                AdjustCharactersPreJoinInstant(leftCharacterContainer, leftCharacterList);
            }
            newCharacter = InstantiateCharacter(leftCharacterPrefab, leftCharacterContainer, leftCharacterList, leftCharacterPool);
        }
        ProcessNewCharacter(newCharacter, args);
    }

    private void AdjustCharactersPreJoinInstant(RectTransform container, List<VNPlusCharacter> characterList)
    {
        GetCharacterAdjustmentValues(container, characterList, 1, out float newHeight, out float posStart);
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            chara.MainRect.sizeDelta = new Vector2(chara.MainRect.sizeDelta.x, newHeight);
            chara.MainRect.anchoredPosition = new Vector2(chara.MainRect.anchoredPosition.x, posStart - (i * newHeight));
        }
    }

    private void ProcessNewCharacter(VNPlusCharacter newCharacter, AddCharacterArgs args)
    {
        var data = args.CharacterData;
        HighlightCharacter(data);
        newCharacter.Data = data;
        newCharacter.NameText = data.mainAlias;
        if (!string.IsNullOrEmpty(args.InitialPose))
        {
            newCharacter.SetPose(args.InitialPose);
        }
        if (!string.IsNullOrEmpty(args.InitialExpression))
        {
            newCharacter.SetExpression(args.InitialExpression);
        }
        characterMap.Add(data.name, newCharacter);
        newCharacter.UpdateSpritePosition();
    }

    private IEnumerator AddCharacterCR(AddCharacterArgs args)
    {
        VNPlusCharacter newCharacter;
        if (args.Column == CharacterColumn.Right)
        {
            if(rightCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListPreJoinCR(rightCharacterContainer, rightCharacterList));
            }
            newCharacter = InstantiateCharacter(rightCharacterPrefab, rightCharacterContainer, rightCharacterList, rightCharacterPool);
        }
        else
        {
            if (leftCharacterList.Count > 0)
            {
                yield return StartCoroutine(AdjustCharacterListPreJoinCR(leftCharacterContainer, leftCharacterList));
            }
            newCharacter = InstantiateCharacter(leftCharacterPrefab, leftCharacterContainer, leftCharacterList, leftCharacterPool);
        }
        ProcessNewCharacter(newCharacter, args);
        yield return newCharacter.PlayJoinTween().WaitForCompletion();
        readyToContinue = true;
    }

    private VNPlusCharacter InstantiateCharacter(GameObject prefab, RectTransform container, List<VNPlusCharacter> characterList, Queue<VNPlusCharacter> pool)
    {
        VNPlusCharacter newCharacter;
        if(pool.Count > 0)
        {
            newCharacter = pool.Dequeue();
            newCharacter.transform.SetAsLastSibling();
            newCharacter.Clear();
            transform.gameObject.SetActive(true);
        }
        else
        {
            newCharacter = Instantiate(prefab, container).GetComponent<VNPlusCharacter>();
        }
        characterList.Add(newCharacter);
        var height = container.rect.height / characterList.Count;
        newCharacter.SetInitialHeight(height);
        newCharacter.SetInitialPos((-height / 2) - ((characterList.Count - 1) * height));
        return newCharacter;
    }

    private IEnumerator AdjustCharacterListPreJoinCR(RectTransform container, List<VNPlusCharacter> characterList)
    {
        characterJoinLeaveTween.Complete();
        GetCharacterAdjustmentValues(container, characterList, 1, out float newHeight, out float posStart);
        AdjustCharacterHeights(characterJoinLeaveTween, characterList, newHeight);
        yield return new WaitForSeconds(characterJoinLeaveTween.Time / 2);
        AdjustCharacterPositions(characterJoinLeaveTween, characterList, posStart, newHeight);
        yield return characterJoinLeaveTween.WaitForCompletion();
    }

    private IEnumerator AdjustCharacterListPostLeaveCR(RectTransform container, List<VNPlusCharacter> characterList)
    {
        characterJoinLeaveTween.Complete();
        GetCharacterAdjustmentValues(container, characterList, 0, out float newHeight, out float posStart);
        AdjustCharacterPositions(characterJoinLeaveTween, characterList, posStart, newHeight);
        yield return new WaitForSeconds(characterJoinLeaveTween.Time / 2);
        AdjustCharacterHeights(characterJoinLeaveTween, characterList, newHeight);
        yield return characterJoinLeaveTween.WaitForCompletion();
    }

    private void GetCharacterAdjustmentValues(RectTransform container, List<VNPlusCharacter> characterList, int extraCharacters, out float newHeight, out float posStart)
    {
        // Extra characters determines the space to leave for additional chracters that are not currently present
        // For example, use 1 extra character when a character is about to join to leave space for the joining character
        newHeight = container.rect.height / (characterList.Count + extraCharacters);
        posStart = -newHeight / 2;
    }

    private IEnumerator MoveSpeakingCharacterToTop(RectTransform container, List<VNPlusCharacter> characterList, VNPlusCharacter character)
    {
        characterList.Remove(character);
        characterList.Insert(0, character);
        character.transform.SetAsLastSibling();
        GetCharacterAdjustmentValues(container, characterList, 0, out float newHeight, out float posStart);
        AdjustCharacterPositions(moveCharaToTopTween, characterList, posStart, newHeight);
        yield return moveCharaToTopTween.WaitForCompletion();
    }

    private void AdjustCharacterHeights(TweenInfo tweenInfo, List<VNPlusCharacter> characterList, float newHeight)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            tweenInfo.Start(chara.MainRect.DOSizeDelta(new Vector2(chara.MainRect.sizeDelta.x, newHeight), tweenInfo.Time), false);
            tweenInfo.Start(chara.CharacterRect.DOAnchorPosY(chara.CalculateSpriteYOffset(newHeight), tweenInfo.Time), false);
        }
    }

    private void AdjustCharacterPositions(TweenInfo tweenInfo, List<VNPlusCharacter> characterList, float posStart, float newHeight)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i]; 
            tweenInfo.Start(chara.MainRect.DOAnchorPosY(posStart - (i * newHeight), tweenInfo.Time), false);
        }
    }

    public override void SetExpression(CharacterData data, string expression)
    {
        if (characterMap.ContainsKey(data.name))// Scene character
        {
            characterMap[data.name].SetExpression(expression);
        }
    }

    public override void SetPose(CharacterData data, string pose)
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
            kvp.Value.Highlighted = data.Any(d => d != null && d.name == kvp.Key);
        }
    }

    public void HighlightCharacter(CharacterData data)
    {
        if (data == null)
            return;
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
        if (dialogItem.CharacterData.Count > 0)// && characterMap.ContainsKey(dialogItem.CharacterData[0].name))
        {
            HighlightCharacter(dialogItem.CharacterData);
        }
        var prefab = GetMessagePrefab(dialogItem.CharacterData, out bool isNarrator);
        DialogBox dialogBox = null;
        if(dialogBoxPool.Count > maxMessages)
        {
            var prefabId = prefab.GetComponent<DialogBox>().ID;
            for(int i = 0; i < dialogBoxPool.Count - maxMessages; ++i)
            {
                if(prefabId == dialogBoxPool[i].ID)
                {
                    dialogBox = dialogBoxPool[i];
                    dialogBoxPool.RemoveAt(i);
                    dialogBoxPool.Add(dialogBox);
                    break;
                }
            }
        }
        if (dialogBox == null)
        {
            dialogBox = Instantiate(prefab, messageContainer).GetComponent<DialogBox>();
            dialogBoxPool.Add(dialogBox);
        }
        dialogBox.transform.SetAsFirstSibling();
        var dialogBoxUI = dialogBox.GetComponent<VNPlusDialogBoxUI>();
        SetCharacterSpecificUI(dialogBoxUI, dialogItem.CharacterData);
        AnimateNewMessageIn(dialogBox, dialogBoxUI, dialogItem, isNarrator);
        // Move speaking character to top if necessary
        var character = dialogItem.CharacterData.Count > 0 ? dialogItem.CharacterData[0] : null;
        if(character != null && characterMap.ContainsKey(character.name))
        {
            var vnChara = characterMap[character.name];
            var charaList = vnChara.Column == CharacterColumn.Left ? leftCharacterList : rightCharacterList;
            if(charaList.Count > 0 && charaList[0] != vnChara)
            {
                var charaContainer = vnChara.Column == CharacterColumn.Left ? leftCharacterContainer : rightCharacterContainer;
                StartCoroutine(MoveSpeakingCharacterToTop(charaContainer, charaList, vnChara));
            }
        }
        return dialogBox;
    }

    private void SetCharacterSpecificUI(VNPlusDialogBoxUI vnPlusUI, List<CharacterData> data)
    {
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

    private GameObject GetMessagePrefab(List<CharacterData> data, out bool isNarrator)
    {
        isNarrator = false;
        if(data.Count > 1)
        {
            throw new System.NotImplementedException("VNPlus mode doesn't currently support multi-character dialog lines");
        }
        if(data.Count <= 0)
        {
            throw new System.NotImplementedException("VNPlus mode doesn't currently support dialog lines with no character data");
        }
        var chara = data[0];
        if (chara == null)
        {
            return randoDialogBoxPrefab;
        }
        if (characterMap.ContainsKey(chara.name))
        {
            return characterMap[chara.name].Column == CharacterColumn.Left ? leftDialogBoxPrefab : rightDialogBoxPrefab;
        }
        isNarrator = true;
        return narratorDialogBoxPrefab;
    }

    private void AnimateNewMessageIn(DialogBox box, VNPlusDialogBoxUI vNPlusDialogUI, DialogItem item, bool isNarrator)
    {
        box.SetupDialogBox(item);
        CompleteMessageTweens();
        LayoutRebuilder.ForceRebuildLayoutImmediate(messageContainer);
        var yTemp = messageContainer.anchoredPosition.y;
        messageContainer.anchoredPosition = new Vector2(messageContainer.anchoredPosition.x, messageContainer.anchoredPosition.y + (box.GetBoxHeight() + messageLayout.spacing));
        messageTween.Start(messageContainer.DOAnchorPosY(yTemp, messageTween.Time));
        if (!isNarrator)
        {
            box.transform.localScale = new Vector3(0, 0, box.transform.localScale.z);
            messageScaleTween.Start(box.transform.DOScale(new Vector3(1, 1, box.transform.localScale.z), messageScaleTween.Time));
        }
        if(lastBoxUI != null)
        {
            lastBoxUI.DoDim(messageFadeTween);
        }
        lastBoxUI = vNPlusDialogUI;
        box.StartDialogScroll();
    }

    private void CompleteMessageTweens()
    {
        messageTween.Complete();
        messageScaleTween.Complete();
        messageFadeTween.Complete();
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }

    public override IEnumerator PlayEnterAnimation()
    {
        contentRoot.localScale = new Vector3(contentRoot.localScale.x, 0, contentRoot.localScale.z);
        if (rightCharacterList.Count + leftCharacterList.Count > 0)
        {
            // Initialize chara scale
            foreach (var chara in leftCharacterList)
            {
                chara.MainRect.localScale = new Vector2(chara.MainRect.localScale.x, 0);
            }
            foreach (var chara in rightCharacterList)
            {
                chara.MainRect.localScale = new Vector2(chara.MainRect.localScale.x, 0);
            }
        }
        enterExitViewTween.Start(contentRoot.DOScaleY(1, enterExitViewTween.Time));
        if (rightCharacterList.Count + leftCharacterList.Count > 0)
        {
            yield return new WaitForSeconds(enterExitStaggerTime);
            yield return StartCoroutine(JoinLeaveAll(true, leftCharacterList, rightCharacterList.Count > 0));
            yield return StartCoroutine(JoinLeaveAll(true, rightCharacterList));
        }
        yield return enterExitViewTween.WaitForCompletion();
    }
    public override IEnumerator PlayExitAnimation(bool isEndOfDialog)
    {
        contentRoot.localScale = new Vector3(contentRoot.localScale.x, 1, contentRoot.localScale.z);
        enterExitViewTween.Complete();
        if (rightCharacterList.Count + leftCharacterList.Count > 0)
        {
            yield return StartCoroutine(JoinLeaveAll(false, rightCharacterList, leftCharacterList.Count > 0));
            yield return StartCoroutine(JoinLeaveAll(false, leftCharacterList));
            yield return new WaitForSeconds(enterExitStaggerTime);
        }
        enterExitViewTween.Start(contentRoot.DOScaleY(0, enterExitViewTween.Time), false);
        yield return enterExitViewTween.WaitForCompletion();
    }



    private IEnumerator JoinLeaveAll(bool join, List<VNPlusCharacter> characterList, bool staggerLast = false)
    {
        float targetScale = join ? 1 : 0;
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            enterExitViewTween.Start(chara.MainRect.DOScaleY(targetScale, enterExitViewTween.Time), false);
            if (staggerLast || i < characterList.Count - 1)
            {
                yield return new WaitForSeconds(enterExitIndividualStaggerTime);
            }
        }
    }

    protected override void SetLocation(string location)
    {
        base.SetLocation(location);
        locationText.text = location;
    }

    protected override void SetDateTime(string dateTime)
    {
        base.SetDateTime(dateTime);
        dateTimeText.text = dateTime;
    }
}
