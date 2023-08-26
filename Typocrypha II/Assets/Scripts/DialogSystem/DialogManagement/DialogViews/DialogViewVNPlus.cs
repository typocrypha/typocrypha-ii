using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class DialogViewVNPlus : DialogViewMessage<DialogItemVNPlus>
{
    public const string narratorName = "narrator";
    private const int maxCharactersPerColumn = 5;
    private const float enterExitStaggerTime = 0.5f;
    private const float enterExitIndividualStaggerTime = 0.05f;

    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private GameObject narratorDialogBoxPrefab;
    [SerializeField] private GameObject randoDialogBoxPrefab;
    [SerializeField] private GameObject rightCharacterPrefab;
    [SerializeField] private GameObject leftCharacterPrefab;
    [SerializeField] private RectTransform rightCharacterContainer;
    [SerializeField] private RectTransform leftCharacterContainer;
    [SerializeField] private RectTransform contentRoot;

    [SerializeField] private TweenInfo moveCharaToTopTween;
    [SerializeField] private TweenInfo characterJoinLeaveTween;
    [SerializeField] private TweenInfo enterExitViewTween;


    public override bool ReadyToContinue => readyToContinue;
    private bool readyToContinue = true;

    private readonly Dictionary<string, VNPlusCharacter> characterMap = new Dictionary<string, VNPlusCharacter>(maxCharactersPerColumn * 2);
    private readonly List<VNPlusCharacter> rightCharacterList = new List<VNPlusCharacter>(maxCharactersPerColumn);
    private readonly List<VNPlusCharacter> leftCharacterList = new List<VNPlusCharacter>(maxCharactersPerColumn);


    private readonly Queue<VNPlusCharacter> rightCharacterPool = new Queue<VNPlusCharacter>(maxCharactersPerColumn);
    private readonly Queue<VNPlusCharacter> leftCharacterPool = new Queue<VNPlusCharacter>(maxCharactersPerColumn);

    public override void CleanUp()
    {
        base.CleanUp();
        // Add the character boxes back to the pool
        foreach (var chara in leftCharacterList)
        {
            chara.MainRect.localScale = new Vector2(chara.MainRect.localScale.x, 0);
            leftCharacterPool.Enqueue(chara);
        }
        foreach (var chara in rightCharacterList)
        {
            chara.MainRect.localScale = new Vector2(chara.MainRect.localScale.x, 0);
            rightCharacterPool.Enqueue(chara);
        }
        leftCharacterList.Clear();
        rightCharacterList.Clear();
        characterMap.Clear();
    }

    #region Character Control

    public override bool RemoveCharacter(CharacterData data)
    {
        // If the character isn't in the scene, simply return
        if (!characterMap.TryGetValue(data.name, out var character))
        {
            return false; // don't wait for completion
        }
        GetColumnData(character.Column, out var container, out var characterList, out var pool);
        PrepareToRemoveCharacter(character, data.name, characterList, pool);
        if (isActiveAndEnabled)
        {
            readyToContinue = false;
            StartCoroutine(RmCharacterCR(character, container, characterList));
            return true; // Wait for completion
        }
        RemoveCharacterInstant(character, container, characterList);
        return false;
    }

    private IEnumerator RmCharacterCR(VNPlusCharacter character, RectTransform container, List<VNPlusCharacter> characterList)
    {
        if (characterList.Count > 0)
        {
            yield return new WaitForSeconds(character.PlayLeaveTween().Time / 2);
            yield return StartCoroutine(AdjustCharacterListPostLeaveCR(container, characterList));
        }
        else
        {
            yield return character.PlayLeaveTween().WaitForCompletion();
        }
        readyToContinue = true;
    }

    private void RemoveCharacterInstant(VNPlusCharacter character, RectTransform container, List<VNPlusCharacter> characterList)
    {
        // Set scale to 0
        var xScale = character.MainRect.localScale.x;
        character.MainRect.localScale = new Vector2(xScale, 0);
        // Adjust remaining characters
        AdjustCharactersPostLeaveInstant(container, characterList);
    }

    private void AdjustCharactersPostLeaveInstant(RectTransform container, List<VNPlusCharacter> characterList)
    {
        GetCharacterAdjustmentValues(container, characterList, 0, out float newHeight, out float posStart);
        for (int i = 0; i < characterList.Count; i++)
        {
            var chara = characterList[i];
            chara.MainRect.sizeDelta = new Vector2(chara.MainRect.sizeDelta.x, newHeight);
            chara.MainRect.anchoredPosition = new Vector2(chara.MainRect.anchoredPosition.x, posStart - (i * newHeight));
        }
    }

    private void PrepareToRemoveCharacter(VNPlusCharacter character, string name, List<VNPlusCharacter> characterList, Queue<VNPlusCharacter> pool)
    {
        characterList.Remove(character);
        pool.Enqueue(character);
        characterMap.Remove(name);
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
        GetColumnData(args.Column, out var container, out var characterList, out var pool, out var prefab);
        if (characterList.Count > 0)
        {
            AdjustCharactersPreJoinInstant(container, characterList);
        }
        var newCharacter = InstantiateCharacter(prefab, container, characterList, pool);
        ProcessNewCharacter(newCharacter, args, true);
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

    private void ProcessNewCharacter(VNPlusCharacter newCharacter, AddCharacterArgs args, bool highlight)
    {
        var data = args.CharacterData;
        if (highlight)
        {
            HighlightCharacter(data);
        }
        else
        {
            newCharacter.Highlighted = false;
        }
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

    private IEnumerator AddCharacterCR(AddCharacterArgs args, bool top = true, bool completePrevious = true)
    {
        GetColumnData(args.Column, out var container, out var characterList, out var pool, out var prefab);
        if (characterList.Count > 0)
        {
            yield return StartCoroutine(AdjustCharacterListPreJoinCR(container, characterList, top, completePrevious));
        }
        var newCharacter = InstantiateCharacter(prefab, container, characterList, pool, top);
        ProcessNewCharacter(newCharacter, args, top);
        yield return newCharacter.PlayJoinTween().WaitForCompletion();
        readyToContinue = true;
    }

    private VNPlusCharacter InstantiateCharacter(GameObject prefab, RectTransform container, List<VNPlusCharacter> characterList, Queue<VNPlusCharacter> pool, bool top = true)
    {
        VNPlusCharacter newCharacter;
        if(pool.Count > 0)
        {
            newCharacter = pool.Dequeue();
            if (top)
            {
                newCharacter.transform.SetAsFirstSibling();
            }
            else
            {
                newCharacter.transform.SetAsLastSibling();
            }
            newCharacter.Clear();
        }
        else
        {
            newCharacter = Instantiate(prefab, container).GetComponent<VNPlusCharacter>();
        }
        // Add character to list
        if (top)
        {
            characterList.Insert(0, newCharacter);
        }
        else
        {
            characterList.Add(newCharacter);
        }
        var height = container.rect.height / characterList.Count;
        newCharacter.SetInitialHeight(height);
        // Set Initial Position
        if (top)
        {
            newCharacter.SetInitialPos(-height / 2); // Top of list
        }
        else
        {
            newCharacter.SetInitialPos((-height / 2) - ((characterList.Count - 1) * height)); // Bottom of list
        }
        return newCharacter;
    }

    private IEnumerator AdjustCharacterListPreJoinCR(RectTransform container, List<VNPlusCharacter> characterList, bool top, bool completePrevious)
    {
        if (completePrevious)
        {
            characterJoinLeaveTween.Complete();
        }
        GetCharacterAdjustmentValues(container, characterList, 1, out float newHeight, out float posStart, top);
        AdjustCharacterHeights(characterJoinLeaveTween, characterList, newHeight);
        yield return new WaitForSeconds(characterJoinLeaveTween.Time / 2);
        AdjustCharacterPositions(characterJoinLeaveTween, characterList, posStart, newHeight);
        yield return new WaitForSeconds(characterJoinLeaveTween.Time / 2);
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

    private void GetCharacterAdjustmentValues(RectTransform container, List<VNPlusCharacter> characterList, int extraCharacters, out float newHeight, out float posStart, bool top = true)
    {
        // Extra characters determines the space to leave for additional chracters that are not currently present
        // For example, use 1 extra character when a character is about to join to leave space for the joining character
        newHeight = container.rect.height / (characterList.Count + extraCharacters);
        if (top)
        {
            posStart = (-newHeight / 2) - (newHeight * extraCharacters);
        }
        else
        {
            posStart = (-newHeight / 2);
        }
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

    public Coroutine MoveCharacter(CharacterData data, CharacterColumn targetColumn, bool top)
    {
        // If the character isn't in the scene, simply return
        if (!characterMap.TryGetValue(data.name, out var character))
        {
            return null; // don't wait for completion
        }
        if (character.Column != targetColumn)
        {
            readyToContinue = false;
            return StartCoroutine(MoveCharacterColumn(character, targetColumn, top));
        }
        return null; // Temp, move character to top/bottom later
    }

    private IEnumerator MoveCharacterColumn(VNPlusCharacter character, CharacterColumn targetColumn, bool top)
    {
        // Cache the data + pose / expression to add back to the target column
        var addArgs = new AddCharacterArgs(character.Data, targetColumn, Vector2.zero, character.Pose, character.Expression);

        // Remove character from current column
        GetColumnData(OtherColumn(targetColumn), out var fromContainer, out var fromList, out var fromPool);
        PrepareToRemoveCharacter(character, character.Data.name, fromList, fromPool);
        StartCoroutine(RmCharacterCR(character, fromContainer, fromList));

        // Wait for half of the leave animation to play (but not any necessary list adjusments)
        yield return new WaitForSeconds(character.JoinTweenTime / 2);

        // Add the character to the target column
        yield return StartCoroutine(AddCharacterCR(addArgs, top, false));
    }

    private void GetColumnData(CharacterColumn column, out RectTransform container, out List<VNPlusCharacter> characterList)
    {
        if (column == CharacterColumn.Right)
        {
            container = rightCharacterContainer;
            characterList = rightCharacterList;
        }
        else // Left column
        {
            container = leftCharacterContainer;
            characterList = leftCharacterList;
        }
    }
    private void GetColumnData(CharacterColumn column, out RectTransform container, out List<VNPlusCharacter> characterList, out Queue<VNPlusCharacter> pool)
    {
        if (column == CharacterColumn.Right)
        {
            pool = rightCharacterPool;
        }
        else // Left column
        {
            pool = leftCharacterPool;
        }
        GetColumnData(column, out container, out characterList);
    }
    private void GetColumnData(CharacterColumn column, out RectTransform container, out List<VNPlusCharacter> characterList, out Queue<VNPlusCharacter> pool, out GameObject prefab)
    {
        if (column == CharacterColumn.Right)
        {
            prefab = rightCharacterPrefab;
        }
        else // Left column
        {
            prefab = leftCharacterPrefab;
        }
        GetColumnData(column, out container, out characterList, out pool);
    }

    private CharacterColumn OtherColumn(CharacterColumn column) => column == CharacterColumn.Right ? CharacterColumn.Left : CharacterColumn.Right;

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
        // Create new message and animate in
        var dialogBox = CreateNewMessage(dialogItem);
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

    protected override GameObject GetMessagePrefab(DialogItemVNPlus dialogItem, List<CharacterData> data, out bool isNarrator)
    {
        isNarrator = false;
        if (data.Count > 1)
        {
            throw new System.NotImplementedException("VNPlus mode doesn't currently support multi-character dialog lines");
        }
        if (data.Count <= 0)
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
    public override IEnumerator PlayExitAnimation(DialogManager.EndType endType)
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
}
