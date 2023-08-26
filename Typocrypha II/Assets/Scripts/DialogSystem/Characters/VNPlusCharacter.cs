using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VNPlusCharacter : MonoBehaviour
{
    private const float scale = 0.99999f;
    private static readonly Color highlightColor = Color.white;
    private static readonly Color noHighlightColor = new Color(noHighlightValue, noHighlightValue, noHighlightValue, 1);
    private static readonly Color nameHighlightColor = new Color(0, 0, 0, 0);
    private static readonly Color nameNoHighlightColor = new Color(0, 0, 0, 1 - noHighlightValue);
    private const float noHighlightValue = 0.65f;
    private const float leftColAdjustment = 0.05f;

    private float nameplateRatio = 0.1f;

    public float JoinTweenTime => joinTween.Time;
    public string Expression { get; private set; }
    public string Pose { get; private set; }
    public RectTransform MainRect => mainRect;
    public RectTransform CharacterRect => poseImage.rectTransform;

    public DialogViewVNPlus.CharacterColumn Column => column;
    [SerializeField] private DialogViewVNPlus.CharacterColumn column;
    [SerializeField] private Image poseImage;
    [SerializeField] private Image expressionImage;
    [SerializeField] private float characterImagePadding = 50f;
    [SerializeField] private AnimationCurve characterYOffsetIntensityCurve;
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
    [SerializeField] private TweenInfo highlightTween;
    [SerializeField] private TweenInfo frameDimTween;
    private TweenInfo exprHighlightTween;
    private TweenInfo nameHighlightTween;
    private TweenInfo rightHighlightTween;
    private TweenInfo leftHighlightTween;
    private Color defaultNameplateOutlineColor;
    private Color defaultNameplateTextColor;
    private Color defaultNameplateColor;

    private Vector2 currentPivot = Vector2.negativeInfinity;
    private bool Flipped
    {
        get => poseImage.transform.localScale.x == -1;
        set
        {
            if (value == Flipped)
                return;
            float xScale = value ? -1 : 1;
            poseImage.transform.localScale = new Vector3(xScale, poseImage.transform.localScale.y);
        }
    }

    private void Awake()
    {
        exprHighlightTween = new TweenInfo(highlightTween);
        nameHighlightTween = new TweenInfo(highlightTween);
        rightHighlightTween = new TweenInfo(frameDimTween);
        leftHighlightTween = new TweenInfo(frameDimTween);
        defaultNameplateColor = nameplateBackground.color;
        defaultNameplateTextColor = nameplateText.color;
        defaultNameplateOutlineColor = nameplateOutline.color;
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
        get => data;
        set
        {
            if (data == value)
                return;
            data = value;
            if(value == null)
            {
                poseImage.enabled = false;
                expressionImage.enabled = false;
                nameplateBackground.color = defaultNameplateColor;
                nameplateOutline.color = defaultNameplateOutlineColor;
                nameplateText.color = defaultNameplateTextColor;
            }
            else
            {
                poseImage.enabled = true;
                expressionImage.enabled = true;
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
            if (isActiveAndEnabled)
            {
                highlightTween.Start(poseImage.DOColor(color, highlightTween.Time));
                exprHighlightTween.Start(expressionImage.DOColor(color, exprHighlightTween.Time));
                nameHighlightTween.Start(nameplateDimmer.DOColor(dimColor, nameHighlightTween.Time));
                rightHighlightTween.Start(rightHighlightDimmer.DOColor(dimColor, rightHighlightTween.Time));
                leftHighlightTween.Start(leftHighlightDimmer.DOColor(dimColor, leftHighlightTween.Time));
            }
            else
            {
                poseImage.color = color;
                expressionImage.color = color;
                nameplateDimmer.color = dimColor;
                rightHighlightDimmer.color = dimColor;
                leftHighlightDimmer.color = dimColor;
            }
        }
    }

    public void SetExpression(string expression)
    {
        if(data == null)
        {
            return;
        }
        string expr = expression.ToLower().Replace("default", "normal");
        if (!data.expressions.ContainsKey(expr))
        {
            Debug.LogError($"{expr} is not a valid expression for {data.mainAlias}");
            return;
        }
        expressionImage.sprite = data.expressions[expr];
        if (currentPivot == Vector2.negativeInfinity)
        {
            UpdateSpritePivot(expressionImage.sprite);
            UpdateSpritePosition();
        }
        Expression = expr;
        // Set Save data
    }

    public void SetPose(string pose)
    {
        if (data == null)
            return;
        if (!data.poses.ContainsKey(pose))
        {
            Debug.LogError($"{pose} is not a valid pose for {data.mainAlias}");
            return;
        }
        var poseData = data.poses[pose];
        poseImage.sprite = poseData.pose;
        // Set flipped
        CharacterData.FacingDirection facing = data.defaultFacingDirection;
        Flipped = facing == CharacterData.FacingDirection.Right && Column == DialogView.CharacterColumn.Right
            || facing == CharacterData.FacingDirection.Left && Column == DialogView.CharacterColumn.Left;
        // Set sprite pivot and position
        if (poseImage.sprite != null)
        {
            poseImage.enabled = true;
            UpdateSpritePivot(poseImage.sprite);
            UpdateSpritePosition();
        }
        else
        {
            poseImage.enabled = false;
        }
        Pose = pose;
        // Set save data
    }

    public void SetInitialPos(float yPos)
    {
        mainRect.anchoredPosition = new Vector2(mainRect.anchoredPosition.x, yPos);
    }

    public void SetInitialHeight(float height)
    {
        mainRect.sizeDelta = new Vector2(mainRect.sizeDelta.x, height);
    }

    public TweenInfo PlayJoinTween()
    {
        mainRect.localScale = new Vector3(mainRect.localScale.x, 0, mainRect.localScale.z);
        joinTween.Start(mainRect.DOScaleY(scale, joinTween.Time));
        return joinTween;
    }

    public TweenInfo PlayLeaveTween()
    {
        joinTween.Start(mainRect.DOScaleY(0, joinTween.Time));
        return joinTween;
    }

    private void UpdateSpritePivot(Sprite spriteToUse)
    {
        currentPivot = new Vector2()
        {
            x = spriteToUse.pivot.x / spriteToUse.rect.width,
            y = spriteToUse.pivot.y / spriteToUse.rect.height
        };
    }

    public void UpdateSpritePosition()
    {
        if(poseImage.sprite != null)
        {
            UpdateSpritePivot(poseImage.sprite);
        }
        else if (expressionImage.sprite != null)
        {
            UpdateSpritePivot(expressionImage.sprite);
        }
        else
        {
            return;
        }
        CharacterRect.anchoredPosition = GetSpriteOffset();
    }

    private Vector2 GetSpriteOffset()
    {
        float xOffset = Mathf.Abs(0.5f - currentPivot.x) * mainRect.sizeDelta.x * (Flipped ? -1 : 1);
        if(currentPivot.x > 0.5f)
        {
            xOffset *= -1;
        }
        if(Column == DialogViewVNPlus.CharacterColumn.Left)
        {
            xOffset += (leftColAdjustment * mainRect.sizeDelta.x);
        }
        float yOffset = CalculateSpriteYOffset();
        return new Vector2(xOffset, yOffset);
    }

    public float CalculateSpriteYOffset(float targetHeight = 0f)
    {
        float heightDiff = targetHeight > 0f ? poseImage.rectTransform.sizeDelta.y - targetHeight
                                             : poseImage.rectTransform.sizeDelta.y - mainRect.sizeDelta.y;

        float intensity = characterYOffsetIntensityCurve.Evaluate(currentPivot.y < 0.75f ? 1f : (heightDiff / poseImage.rectTransform.sizeDelta.y));

        return heightDiff > 0f ? poseImage.rectTransform.sizeDelta.y * (1.0f - currentPivot.y) * intensity : 0f;
    }   

    public void Clear()
    {
        Data = null;
    }
}
