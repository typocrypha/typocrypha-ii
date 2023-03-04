﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Dialog view for positionable text bubbles.
/// </summary>
public class DialogViewBubble : DialogView
{
    public static readonly Vector2 nullAbsolutePos = new Vector2(-100, -100);
    private static readonly Vector2Int nullGridPos = new Vector2Int(-1, -1);
    [SerializeField] private DialogBox defaultDialogBox;
    [SerializeField] private List<DialogBox> dialogBoxGrid;
    [SerializeField] private TweenInfo showHideBoxTween;

    private readonly List<Vector2> initialPositions = new List<Vector2>(6);
    private Vector2Int lastBoxPosition = nullGridPos;
    private Vector2 lastBoxAbsolutePosition = nullAbsolutePos;
    private CharacterData lastCharacterData = null;

    public override bool ReadyToContinue => readyToContinue;
    private bool readyToContinue = true;

    protected AllyBattleBoxManager CharacterManager => AllyBattleBoxManager.instance;
    protected bool CharacterManagerUnavailable => CharacterManager == null;

    protected bool IsBoxShowing => lastBoxPosition != nullGridPos;

    private void Awake()
    {
        initialPositions.Clear();
        foreach(var box in dialogBoxGrid)
        {
            initialPositions.Add(box.GetComponent<RectTransform>().anchoredPosition);
        }
    }

    private void ResetLastBox()
    {
        lastBoxPosition = nullGridPos;
        lastBoxAbsolutePosition = nullAbsolutePos;
    }

    private static int GridIndex(Vector2Int index)
    {
        return (index.x * 3) + index.y;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }

    private DialogBox GetBoxFromGrid(Vector2Int index)
    {
        try
        {
            return dialogBoxGrid[GridIndex(index)];
        }
        catch
        {
            return defaultDialogBox;
        }
    }

    private Vector2 GetFixedPositionFromGrid(Vector2Int index)
    {
        try
        {
            return initialPositions[GridIndex(index)];
        }
        catch
        {
            Debug.LogError($"Can't find fixed position for index {index}, return box at index 0");
            return initialPositions[0];
        }
    }

    /// <summary>
    /// Create and start single or multiple speech bubbles.
    /// </summary>
    public override DialogBox PlayDialog(DialogItem data)
    {
        var bubbleData = data as DialogItemBubble;
        var dialogBox = GetBoxFromGrid(bubbleData.GridPosition);
        var dialogBoxUI = dialogBox.GetComponent<BubbleDialogBoxUI>();
        var character = bubbleData.CharacterData.Count > 0 ? bubbleData.CharacterData[0] : null; 
        // If same position and box, just continue playing in that box
        if (bubbleData.GridPosition == lastBoxPosition && bubbleData.AbsolutePosition == lastBoxAbsolutePosition && character == lastCharacterData)
        {
            dialogBoxUI.Refresh();
            dialogBox.SetupAndStartDialogBox(data);
            return dialogBox;
        }
        // Else, hide last box and show new box
        StartCoroutine(AnimateNewMessageIn(dialogBox, character, dialogBoxUI, bubbleData, bubbleData.GridPosition, bubbleData.AbsolutePosition));
        return dialogBox;
    }

    private IEnumerator AnimateNewMessageIn(DialogBox box, CharacterData character, BubbleDialogBoxUI ui, DialogItem data, Vector2Int gridPos, Vector2 absPos)
    {
        readyToContinue = false;
        if (IsBoxShowing)
        {
            yield return StartCoroutine(HideCurrentBox(false));
        }
        lastBoxPosition = gridPos;
        lastBoxAbsolutePosition = absPos;
        lastCharacterData = character;

        // Setup dialog box
        box.SetupDialogBox(data);
        ui.Bind(character);

        // Enable ans start new dialog box
        box.transform.localScale = Vector2.zero;
        box.gameObject.SetActive(true);
        box.StartDialogScroll();
        readyToContinue = true;

        var rect = box.GetComponent<RectTransform>();
        // set position of new dialog box
        if (absPos != nullAbsolutePos) // absolute pos (in relative screen coords)
        {
            // Normalized World Positioning (Top Left)
            var res = new Vector2(1280, 720);
            rect.anchoredPosition = (absPos * res) - (rect.pivot * res);
        }
        else // fixed position
        {
            rect.anchoredPosition = GetFixedPositionFromGrid(gridPos);
        }

        // show new dialog box
        showHideBoxTween.Start(box.transform.DOScale(Vector2.one, showHideBoxTween.Time));
        yield return showHideBoxTween.WaitForCompletion();
    }

    private IEnumerator HideCurrentBox(bool resetLastBox = true)
    {
        if (!IsBoxShowing)
            yield break;
        // hide Current
        var currentBox = GetBoxFromGrid(lastBoxPosition);
        showHideBoxTween.Start(currentBox.transform.DOScale(Vector2.zero, showHideBoxTween.Time));
        yield return showHideBoxTween.WaitForCompletion();
        currentBox.gameObject.SetActive(false);
        if (resetLastBox)
        {
            ResetLastBox();
        }
    }

    public override void CleanUp()
    {
        ResetLastBox();
        foreach (var box in dialogBoxGrid)
        {
            box.gameObject.SetActive(false);
        }
    }

    public override Coroutine Clear()
    {
        return StartCoroutine(HideCurrentBox());
    }

    public override bool AddCharacter(AddCharacterArgs args)
    {
        if(CharacterManagerUnavailable || CharacterManager.IsCurrentCharacter(args.CharacterData))
        {
            return false;
        }
        if (isActiveAndEnabled)
        {
            readyToContinue = false;
            StartCoroutine(AddCharacterCR(args.CharacterData, args.InitialExpression, args.InitialPose));
            return true;
        }
        CharacterManager.SetCharacterInstant(args.CharacterData, args.InitialExpression, args.InitialPose);
        return false;
    }

    private IEnumerator AddCharacterCR(CharacterData data, string initialExpr, string initialPose)
    {
        if (IsBoxShowing)
        {
            yield return StartCoroutine(HideCurrentBox());
        }
        yield return CharacterManager.AddCharacter(data, initialExpr, initialPose);
        readyToContinue = true;
    }

    public override bool RemoveCharacter(CharacterData data)
    {
        if (CharacterManagerUnavailable)
        {
            return false;
        }
        readyToContinue = false;
        StartCoroutine(RemoveCharacterCR());
        return true;
    }

    private IEnumerator RemoveCharacterCR()
    {
        if (IsBoxShowing)
        {
            yield return StartCoroutine(HideCurrentBox());
        }
        yield return CharacterManager.HideCharacter();
        readyToContinue = true;
    }

    public override void SetExpression(CharacterData data, string expression)
    {
        if (CharacterManagerUnavailable)
            return;
        CharacterManager.SetExpression(expression);
    }

    public override void SetPose(CharacterData data, string pose)
    {
        if (CharacterManagerUnavailable)
            return;
        CharacterManager.SetPose(pose);
    }

    public override IEnumerator PlayExitAnimation(bool isEndOfDialog)
    {
        if (isEndOfDialog)
        {
            yield break;
        }
        yield return StartCoroutine(RemoveCharacterCR());
    }
}
