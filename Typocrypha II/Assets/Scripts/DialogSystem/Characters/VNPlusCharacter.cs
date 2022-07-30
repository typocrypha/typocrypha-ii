using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VNPlusCharacter : MonoBehaviour
{
    private static readonly Color highlightColor = Color.white;
    private static readonly Color noHighlightColor = new Color(noHighlightValue, noHighlightValue, noHighlightValue, 1);
    private static readonly Color nameHighlightColor = new Color(0, 0, 0, 0);
    private static readonly Color nameNoHighlightColor = new Color(0, 0, 0, 1 - noHighlightValue);
    private const float noHighlightValue = 0.65f;
    private const float leftColAdjustment = 0.1f;

    private float nameplateRatio = 0.1f;

    public DialogViewVNPlus.CharacterColumn Column => column;
    [SerializeField] private DialogViewVNPlus.CharacterColumn column;
    [SerializeField] private Image poseImage;
    [SerializeField] private Image expressionImage;
    [SerializeField] private Image leftHighlightImage;
    [SerializeField] private Image leftHighlightDimmer;
    [SerializeField] private Image rightHighlightImage;
    [SerializeField] private Image rightHighlightDimmer;
    [SerializeField] private Image nameplateBackground;
    [SerializeField] private float nameplateBackgroundOpacity = 0.75f;
    [SerializeField] private Image nameplateDimmer;
    [SerializeField] private Image nameplateOutline;
    [SerializeField] private TextMeshProUGUI nameplateText;
    [SerializeField] private RectTransform mainRect;
    [SerializeField] private TweenInfo joinTween;
    [SerializeField] private TweenInfo adjustSizeTween;
    [SerializeField] private TweenInfo adjustPosTween;
    [SerializeField] private TweenInfo highlightTween;
    [SerializeField] private TweenInfo frameDimTween;
    private TweenInfo exprHighlightTween;
    private TweenInfo nameHighlightTween;
    private TweenInfo rightHighlightTween;
    private TweenInfo leftHighlightTween;

    private void Awake()
    {
        exprHighlightTween = new TweenInfo(highlightTween);
        nameHighlightTween = new TweenInfo(highlightTween);
        rightHighlightTween = new TweenInfo(frameDimTween);
        leftHighlightTween = new TweenInfo(frameDimTween);
    }

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
            nameplateOutline.color = data.characterColorDark;

            Color nameplateCol = data.characterColorLight;
            nameplateCol.a = nameplateBackgroundOpacity;
            nameplateBackground.color = nameplateCol;

            leftHighlightImage.color = data.characterHighlightColorLeft;
            rightHighlightImage.color = data.characterHighlightColorRight;
        }
    }
    private CharacterData data;
    private bool isHighlighted = true;

    public bool Highlighted
    {
        get => isHighlighted;
        set
        {
            if (isHighlighted == value)
                return;
            isHighlighted = value;
            var color = value ? highlightColor : noHighlightColor;
            var dimColor = value ? nameHighlightColor : nameNoHighlightColor;
            highlightTween.Start(poseImage.DOColor(color, highlightTween.Time));
            exprHighlightTween.Start(expressionImage.DOColor(color, exprHighlightTween.Time));
            nameHighlightTween.Start(nameplateDimmer.DOColor(dimColor, nameHighlightTween.Time));
            rightHighlightTween.Start(rightHighlightDimmer.DOColor(dimColor, rightHighlightTween.Time));
            leftHighlightTween.Start(leftHighlightDimmer.DOColor(dimColor, leftHighlightTween.Time));
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

    public TweenInfo DoAdjustPosTween(float yPos)
    {
        adjustPosTween.Start(mainRect.DOAnchorPosY(yPos, adjustPosTween.Time));
        return adjustPosTween;
    }

    public void SetInitialHeight(float height)
    {
        mainRect.sizeDelta = new Vector2(mainRect.sizeDelta.x, height);
    }

    public TweenInfo DoAdjustHeightTween(float height)
    {
        adjustSizeTween.Start(mainRect.DOSizeDelta(new Vector2(mainRect.sizeDelta.x, height), adjustSizeTween.Time));
        return adjustSizeTween;
    }

    public TweenInfo PlayJoinTween()
    {
        float scale = mainRect.localScale.y;
        mainRect.localScale = new Vector3(mainRect.localScale.x, 0, mainRect.localScale.z);
        joinTween.Start(mainRect.DOScaleY(scale, joinTween.Time));
        return joinTween;
    }

    public TweenInfo PlayLeaveTween()
    {
        joinTween.Start(mainRect.DOScaleY(0, joinTween.Time));
        return joinTween;
    }
}
