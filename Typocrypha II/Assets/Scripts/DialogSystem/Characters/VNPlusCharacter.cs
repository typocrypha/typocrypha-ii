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

    public DialogViewVNPlus.CharacterColumn Column => column;
    [SerializeField] private DialogViewVNPlus.CharacterColumn column;
    [SerializeField] private Image poseImage;
    [SerializeField] private Image expressionImage;
    [SerializeField] private Image nameplateImage;
    [SerializeField] private TextMeshProUGUI nameplateText;
    [SerializeField] private RectTransform mainRect;

    [SerializeField] private Ease joinEase;
    [SerializeField] private bool useCustomJoinEase;
    [SerializeField] private AnimationCurve customJoinEase;
    [SerializeField] private float joinTweenTime = 0.25f;

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
            // Set save data
        }
        else
        {
            Debug.LogError("No such pose:" + pose);
        }
    }

    public void PlayJoinTween()
    {
        float scale = mainRect.localScale.y;
        mainRect.localScale = new Vector3(mainRect.localScale.x, 0, mainRect.localScale.z);
        var tween = mainRect.DOScaleY(scale, joinTweenTime);
        // Play animation
        if (useCustomJoinEase)
        {
            tween.SetEase(customJoinEase);
        }
        else
        {
            tween.SetEase(joinEase);
        }
    }

}
