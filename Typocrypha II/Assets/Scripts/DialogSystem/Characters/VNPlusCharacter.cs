using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VNPlusCharacter : MonoBehaviour
{
    private static readonly Color highlightColor = Color.white;
    private static readonly Color noHighlightColor = Color.gray;
    private const float leftColAdjustment = 0.1f;

    public DialogViewVNPlus.CharacterColumn Column => column;
    [SerializeField] private DialogViewVNPlus.CharacterColumn column;
    [SerializeField] private Image poseImage;
    [SerializeField] private Image expressionImage;
    [SerializeField] private Image nameplateImage;
    [SerializeField] private TextMeshProUGUI nameplateText;
    [SerializeField] private RectTransform mainRect;
    [Header("Join Tween")]
    [SerializeField] private Ease joinEase;
    [SerializeField] private bool useCustomJoinEase;
    [SerializeField] private AnimationCurve customJoinEase;
    [SerializeField] private float joinTweenTime = 0.25f;
    [Header("Adjustment Tweens")]
    [SerializeField] private Ease adjustSizeEase;
    [SerializeField] private bool useCustomAdjustSizeEase;
    [SerializeField] private AnimationCurve customAdjustSizeEase;
    [SerializeField] private Ease adjustPosEase;
    [SerializeField] private bool useCustomAdjustPosEase;
    [SerializeField] private AnimationCurve customAdjustPosEase;
    [SerializeField] private float adjustTweenTime = 0.25f;
    public float AdjustTweenTime => adjustTweenTime;

    public string NameText 
    { 
        get => nameplateText.text;
        set
        {
            nameplateText.text = value;
        }
    }

    public CharacterData Data 
    {
        set
        {
            if (data != null && data.name == value.name)
                return;
            data = value;
            SetPose(DialogCharacterManager.defaultPose);
            SetExpression(DialogCharacterManager.defaultExpr);
            nameplateText.color = data.characterColorDark;
            nameplateImage.color = data.characterColorLight;
        }
    }
    private CharacterData data;

    public bool Highlighted
    {
        set
        {
            if (value)
            {
                poseImage.color = highlightColor;
                expressionImage.color = highlightColor;
            }
            else
            {
                poseImage.color = noHighlightColor;
                expressionImage.color = noHighlightColor;
            }
        }
    }

    public void SetExpression(string expression)
    {
        string expr = expression.ToLower().Replace("default", "normal");
        if (data.expressions.ContainsKey(expr))
        {
            expressionImage.sprite = data.expressions[expr];
            // Set Save data
        }
        else
        {
            Debug.LogError("No such expression:" + expr);
        }
    }

    public void SetPose(string pose)
    {
        if (data.poses.ContainsKey(pose))
        {
            var poseData = data.poses[pose];
            poseImage.sprite = poseData.pose;
            // Set position
            float xValue = poseData.xCenterNormalized;
            if(column == DialogViewVNPlus.CharacterColumn.Left)
            {
                xValue = Mathf.Abs(1 - xValue) + leftColAdjustment;
            }
            poseImage.rectTransform.anchorMin = new Vector2(xValue, poseData.yHeadTopNormalized);
            poseImage.rectTransform.anchorMax = new Vector2(xValue, poseData.yHeadTopNormalized);
            // Set save data
        }
        else
        {
            Debug.LogError("No such pose:" + pose);
        }
    }

    public void SetInitialPos(float yPos)
    {
        mainRect.anchoredPosition = new Vector2(mainRect.anchoredPosition.x, yPos);
    }

    public Tween DoAdjustPosTween(float yPos)
    {
        var tween = mainRect.DOAnchorPosY(yPos, adjustTweenTime);
        CustomTweenEase(tween, adjustPosEase, customAdjustPosEase, useCustomAdjustPosEase);
        return tween;
    }

    public void SetInitialHeight(float height)
    {
        mainRect.sizeDelta = new Vector2(mainRect.sizeDelta.x, height);
    }

    public Tween DoAdjustHeightTween(float height)
    {
        var tween = mainRect.DOSizeDelta(new Vector2(mainRect.sizeDelta.x, height), adjustTweenTime);
        CustomTweenEase(tween, adjustSizeEase, customAdjustSizeEase, useCustomAdjustSizeEase);
        return tween;
    }

    public Tween PlayJoinTween()
    {
        float scale = mainRect.localScale.y;
        mainRect.localScale = new Vector3(mainRect.localScale.x, 0, mainRect.localScale.z);
        var tween = mainRect.DOScaleY(scale, joinTweenTime);
        CustomTweenEase(tween, joinEase, customJoinEase, useCustomJoinEase);
        return tween;
    }

    public Tween PlayLeaveTween()
    {
        var tween = mainRect.DOScaleY(0, joinTweenTime);
        CustomTweenEase(tween, joinEase, customJoinEase, useCustomJoinEase);
        return tween;
    }

    private void CustomTweenEase(Tween tween, Ease ease, AnimationCurve customEase, bool useCustom)
    {
        if (useCustom)
        {
            tween.SetEase(customEase);
        }
        else
        {
            tween.SetEase(ease);
        }
    }

}
