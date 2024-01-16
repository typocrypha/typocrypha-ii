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
    private static readonly WaitForSeconds multiJoinLeaveStaggerYielder = new WaitForSeconds(0.1f);

    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private GameObject narratorDialogBoxPrefab;
    [SerializeField] private GameObject randoDialogBoxPrefab;
    [SerializeField] private GameObject rightCharacterPrefab;
    [SerializeField] private GameObject leftCharacterPrefab;
    [SerializeField] private GameObject embeddedImagePrefab;
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
            StartCoroutine(RmCharacterCR(character, container, characterList, true, true));
            return true; // Wait for completion
        }
        RemoveCharacterInstant(character, container, characterList);
        return false;
    }

    public override bool RemoveCharacterMulti(IReadOnlyList<CharacterData> args)
    {
        var charactersToRemove = args.Where(arg => characterMap.ContainsKey(arg.name)).ToList();
        if (charactersToRemove.Count < 1)
        {
            return false;
        }
        else if (charactersToRemove.Count == 1)
        {
            return RemoveCharacter(args[0]);
        }
        var rightCharactersToRemove = new List<VNPlusCharacter>(charactersToRemove.Count);
        var leftCharactersToRemove = new List<VNPlusCharacter>(charactersToRemove.Count);
        foreach(var charaData in charactersToRemove)
        {
            var character = characterMap[charaData.name];
            var column = character.Column;
            if(column == CharacterColumn.Right)
            {
                PrepareToRemoveCharacter(character, charaData.name, rightCharacterList, rightCharacterPool);
                rightCharactersToRemove.Add(character);
            }
            else
            {
                PrepareToRemoveCharacter(character, charaData.name, leftCharacterList, leftCharacterPool);
                leftCharactersToRemove.Add(character);
            }
        }
        if (isActiveAndEnabled)
        {
            readyToContinue = false;
            if(rightCharactersToRemove.Count <= 0)
            {
                GetColumnData(CharacterColumn.Left, out var container, out var characterList, out var pool);
                StartCoroutine(RmCharacterCRMulti(leftCharactersToRemove, container, characterList, true, true));
            }
            else if(leftCharactersToRemove.Count <= 0)
            {
                GetColumnData(CharacterColumn.Right, out var container, out var characterList, out var pool);
                StartCoroutine(RmCharacterCRMulti(rightCharactersToRemove, container, characterList, true, true));
            }
            else
            {
                StartCoroutine(RmCharacterCRMultiBothColumns(rightCharactersToRemove, leftCharactersToRemove));
            }
            return true;
        }
        return base.RemoveCharacterMulti(args);
    }

    private IEnumerator RmCharacterCR(VNPlusCharacter character, RectTransform container, List<VNPlusCharacter> characterList, bool completePrevious, bool last)
    {
        if (characterList.Count > 0)
        {
            yield return new WaitForSeconds(character.PlayLeaveTween().Time / 2);
            yield return StartCoroutine(AdjustCharacterListPostLeaveCR(container, characterList, completePrevious));
        }
        else
        {
            yield return character.PlayLeaveTween().WaitForCompletion();
        }
        if (last)
        {
            readyToContinue = true;
        }
    }

    private IEnumerator RmCharacterCRMulti(List<VNPlusCharacter> charactersToRemove, RectTransform container, List<VNPlusCharacter> characterList, bool completePrevious, bool last)
    {
        if (characterList.Count > 0)
        {
            TweenInfo lastTween = null;
            foreach (var character in charactersToRemove)
            {
                lastTween = character.PlayLeaveTween();
                yield return multiJoinLeaveStaggerYielder;
            }
            yield return new WaitForSeconds(lastTween.Time / 2);
            yield return StartCoroutine(AdjustCharacterListPostLeaveCR(container, characterList, completePrevious));
        }
        else
        {
            YieldInstruction yielder = null;
            foreach(var character in charactersToRemove)
            {
                yielder = character.PlayLeaveTween().WaitForCompletion();
                yield return multiJoinLeaveStaggerYielder;
            }
            yield return yielder;
        }
        if (last)
        {
            readyToContinue = true;
        }
    }


    private IEnumerator RmCharacterCRMultiBothColumns(List<VNPlusCharacter> rightCharactersToRemove, List<VNPlusCharacter> leftCharactersToRemove)
    {
        var coroutines = new List<Coroutine>(2);
        if(rightCharactersToRemove.Count == 1)
        {
            GetColumnData(CharacterColumn.Right, out var container, out var characterList, out _);
            coroutines.Add(StartCoroutine(RmCharacterCR(rightCharactersToRemove[0], container, characterList, false, false)));
        }
        else
        {
            GetColumnData(CharacterColumn.Right, out var container, out var characterList, out _);
            coroutines.Add(StartCoroutine(RmCharacterCRMulti(rightCharactersToRemove, container, characterList, false, false)));
        }
        yield return multiJoinLeaveStaggerYielder;
        if (leftCharactersToRemove.Count == 1)
        {
            GetColumnData(CharacterColumn.Left, out var container, out var characterList, out _);
            coroutines.Add(StartCoroutine(RmCharacterCR(leftCharactersToRemove[0], container, characterList, false, false)));
        }
        else
        {
            GetColumnData(CharacterColumn.Left, out var container, out var characterList, out _);
            coroutines.Add(StartCoroutine(RmCharacterCRMulti(leftCharactersToRemove, container, characterList, false, false)));
        }
        foreach(var cr in coroutines)
        {
            yield return cr;
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

    public override bool AddCharacterMulti(IReadOnlyList<AddCharacterArgs> args)
    {
        var charactersToAdd = args.Where(arg => !characterMap.ContainsKey(arg.CharacterData.name)).ToArray();
        if(charactersToAdd.Length < 1)
        {
            return false;
        }
        else if(charactersToAdd.Length == 1)
        {
            return AddCharacter(args[0]);
        }
        if (isActiveAndEnabled)
        {
            readyToContinue = false;
            StartCoroutine(AddCharacterMultiCR(args));
            return true;
        }
        return base.AddCharacterMulti(args);
    }

    private void AddCharacterInstant(AddCharacterArgs args)
    {
        GetColumnData(args.Column, out var container, out var characterList, out var pool, out var prefab);
        if (characterList.Count > 0)
        {
            AdjustCharactersPreJoinInstant(container, characterList);
        }
        var newCharacter = InstantiateCharacter(prefab, container, characterList, pool, 0);
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
            yield return StartCoroutine(AdjustCharacterListPreJoinCR(container, characterList, top, completePrevious, 1));
        }
        var newCharacter = InstantiateCharacter(prefab, container, characterList, pool, top ? 0 : characterList.Count);
        ProcessNewCharacter(newCharacter, args, top);
        yield return newCharacter.PlayJoinTween().WaitForCompletion();
        readyToContinue = true;
    }

    private IEnumerator AddCharacterMultiCR(IReadOnlyList<AddCharacterArgs> args, bool top = true, bool completePrevious = true)
    {
        GetColumnData(args[0].Column, out var container, out var characterList, out var pool, out var prefab);
        if (characterList.Count > 0)
        {
            yield return StartCoroutine(AdjustCharacterListPreJoinCR(container, characterList, top, completePrevious, args.Count));
        }
        var newCharacters = new List<VNPlusCharacter>(args.Count);
        for (int i = 0; i < args.Count; i++)
        {
            newCharacters.Add(CreateCharacter(prefab, container, characterList, pool, i));
        }
        for (int i = 0; i < newCharacters.Count; i++)
        {
            var arg = args[i];
            var newCharacter = newCharacters[i];
            InitializeCharacter(newCharacter, container, characterList, i);
            ProcessNewCharacter(newCharacter, arg, i == 0);
            newCharacter.PreJoinTween();
        }
        YieldInstruction yielder = null;
        foreach (var newCharacter in newCharacters)
        {
            yielder = newCharacter.PlayJoinTween().WaitForCompletion();
            yield return multiJoinLeaveStaggerYielder;
        }
        yield return yielder;
        readyToContinue = true;
    }

    private VNPlusCharacter InstantiateCharacter(GameObject prefab, RectTransform container, List<VNPlusCharacter> characterList, Queue<VNPlusCharacter> pool, int index)
    {
        VNPlusCharacter newCharacter = CreateCharacter(prefab, container, characterList, pool, index);
        InitializeCharacter(newCharacter, container, characterList, index);
        return newCharacter;
    }

    private VNPlusCharacter CreateCharacter(GameObject prefab, RectTransform container, List<VNPlusCharacter> characterList, Queue<VNPlusCharacter> pool, int index)
    {
        VNPlusCharacter newCharacter;
        if (pool.Count > 0)
        {
            newCharacter = pool.Dequeue();
            newCharacter.transform.SetSiblingIndex(index);
            newCharacter.Clear();
        }
        else
        {
            newCharacter = Instantiate(prefab, container).GetComponent<VNPlusCharacter>();
        }
        // Add character to list
        characterList.Insert(index, newCharacter);
        return newCharacter;
    }

    private void InitializeCharacter(VNPlusCharacter newCharacter, RectTransform container, List<VNPlusCharacter> characterList, int index)
    {
        var height = container.rect.height / characterList.Count;
        newCharacter.SetInitialHeight(height);
        // Set Initial Position
        newCharacter.SetInitialPos((-height / 2) - (index * height));
    }

    private IEnumerator AdjustCharacterListPreJoinCR(RectTransform container, List<VNPlusCharacter> characterList, bool top, bool completePrevious, int numJoining)
    {
        if (completePrevious)
        {
            characterJoinLeaveTween.Complete();
        }
        GetCharacterAdjustmentValues(container, characterList, numJoining, out float newHeight, out float posStart, top);
        AdjustCharacterHeights(characterJoinLeaveTween, characterList, newHeight);
        yield return new WaitForSeconds(characterJoinLeaveTween.Time / 2);
        AdjustCharacterPositions(characterJoinLeaveTween, characterList, posStart, newHeight);
        yield return new WaitForSeconds(characterJoinLeaveTween.Time / 2);
    }

    private IEnumerator AdjustCharacterListPostLeaveCR(RectTransform container, List<VNPlusCharacter> characterList, bool completePrevious)
    {
        if (completePrevious)
        {
            characterJoinLeaveTween.Complete();
        }
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
        StartCoroutine(RmCharacterCR(character, fromContainer, fromList, true, false));

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

    protected override GameObject GetImagePrefab()
    {
        return embeddedImagePrefab;
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

    public void Shake(float intensity, float duration)
    {
        contentRoot.DOShakeAnchorPos(duration, intensity, 64); //TODO: fix magic number
    }
}
