using DG.Tweening;
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

    public VNPlusCharacter CurrentChar => rightColumnCharas[rightColumnIndex];
    private int rightColumnIndex = 0;
    public bool AllCharactersHidden { get; private set; } = false;

    // Toggle between 0 and 1
    private static int Toggle(int arg) => Mathf.Abs(arg - 1);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetCharacterInstant(CharacterData data)
    {
        CurrentChar.Data = data;
        CurrentChar.NameText = data.mainAlias;
    }

    public YieldInstruction AddCharacter(CharacterData data)
    {
        if (CurrentChar.Data == data)
            return null;
        if (AllCharactersHidden)
        {
            SetCharacterInstant(data);
            return ShowCharacter();
        }
        var oldChar = CurrentChar;
        // Set to ne character
        rightColumnIndex = Toggle(rightColumnIndex);
        SetCharacterInstant(data);
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
