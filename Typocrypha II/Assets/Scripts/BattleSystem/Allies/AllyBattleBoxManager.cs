﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note, currently doesn't support two columns, but could in the future
public class AllyBattleBoxManager : MonoBehaviour
{
    public static AllyBattleBoxManager instance;
    [SerializeField] private VNPlusCharacter[] rightColumnCharas;
    [SerializeField] private RectTransform onScreenPos;
    [SerializeField] private RectTransform offScreenPos;
    [SerializeField] private TweenInfo joinTweenInfo;

    public bool HasActiveCharacter => !AllCharactersHidden && CurrentChar.Data != null;
    public VNPlusCharacter CurrentChar => rightColumnCharas[rightColumnIndex];
    private int rightColumnIndex = 0;
    public bool AllCharactersHidden { get; private set; } = false;
    public CharacterData BattleAllyData { get; private set; }
    private string battleAllyPose = "";
    private string battleAllyExpr = "";

    public void SetBattleAllyData(CharacterData data, string expr = "", string pose = "")
    {
        BattleAllyData = data;
        battleAllyExpr = expr;
        battleAllyPose = pose;
    }

    public bool IsCurrentCharacter(CharacterData data)
    {
        return HasActiveCharacter && CurrentChar.Data == data;
    }

    // Toggle between 0 and 1
    private static int Toggle(int arg) => Mathf.Abs(arg - 1);

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SetCharacterInstant(CharacterData data, string initialExpr = "", string initialPose = "")
    {
        CurrentChar.Data = data;
        CurrentChar.NameText = data?.mainAlias ?? "No Ally";
        if(data == null)
        {
            return;
        }
        if (!string.IsNullOrEmpty(initialPose))
        {
            CurrentChar.SetPose(initialPose);
        }
        if (!string.IsNullOrEmpty(initialExpr))
        {
            CurrentChar.SetExpression(initialExpr);
        }
    }

    public void SetBattleAllyCharacterInstant()
    {
        SetCharacterInstant(BattleAllyData, battleAllyExpr, battleAllyPose);
    }

    public YieldInstruction AddCharacter(CharacterData data, string initialExpr = "", string initialPose = "")
    {
        if (AllCharactersHidden)
        {
            SetCharacterInstant(data, initialExpr, initialPose);
            return ShowCharacter();
        }
        if (CurrentChar.Data == data)
        {
            // Character data will be the same, but expression might be different
            SetCharacterInstant(data, initialExpr, initialPose);
            return null;
        }
        var oldChar = CurrentChar;
        // Set to new character
        rightColumnIndex = Toggle(rightColumnIndex);
        SetCharacterInstant(data, initialExpr, initialPose);
        // Set new Character to be layered above old one
        CurrentChar.transform.SetAsLastSibling();
        // Remove old character
        PlayJoinLeaveTween(oldChar, offScreenPos, true);
        // Join New Character
        PlayJoinLeaveTween(CurrentChar, onScreenPos, false);
        return joinTweenInfo.WaitForCompletion();
    }

    public YieldInstruction HideCharacter()
    {
        if (AllCharactersHidden)
        {
            return null;
        }
        // Move current character offscreen
        PlayJoinLeaveTween(CurrentChar, offScreenPos, true);
        AllCharactersHidden = true;
        return joinTweenInfo.WaitForCompletion();
    }

    public YieldInstruction ShowCharacter()
    {
        if (!AllCharactersHidden)
        {
            return null;
        }
        // Move current character offscreen
        PlayJoinLeaveTween(CurrentChar, onScreenPos, true);
        AllCharactersHidden = false;
        return joinTweenInfo.WaitForCompletion();
    }

    public YieldInstruction ShowBattleAlly()
    {
        return AddCharacter(BattleAllyData, battleAllyExpr, battleAllyPose);
    }

    private void PlayJoinLeaveTween(VNPlusCharacter chara, RectTransform target, bool complete)
    {
        joinTweenInfo.Start(chara.MainRect.DOAnchorPosX(target.anchoredPosition.x, joinTweenInfo.Time), complete);
    }

    public void SetExpression(string expression)
    {
        CurrentChar.SetExpression(expression);
    }

    public void SetPose(string pose)
    {
        CurrentChar.SetPose(pose);
    }

}
