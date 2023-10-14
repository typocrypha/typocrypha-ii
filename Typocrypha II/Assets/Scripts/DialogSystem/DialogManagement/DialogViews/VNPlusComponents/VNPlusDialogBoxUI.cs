﻿using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VNPlusDialogBoxUI : MonoBehaviour
{
    private const int nonBorderComponents = 2;
    private const float dimAmount = 0.55f;
    [SerializeField] private Color dimColor = new Color(dimAmount, dimAmount, dimAmount, 1);
    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] borderImages;
    [SerializeField] private Image arrowImage;
    [SerializeField] private CanvasGroup[] dimmingCanvasGroups;
    [SerializeField] private DialogContinueIndicator continueIndicator;

    private Color initialColor = Color.clear;

    public void Bind(CharacterData character)
    {
        iconImage.color = Color.white;
        foreach(var canvasGroup in dimmingCanvasGroups)
        {
            canvasGroup.alpha = 1;
        }
        // Generic Character functionality
        if (character == null)
        {
            if(initialColor == Color.clear)
            {
                initialColor = borderImages[0].color;
            }
            else
            {
                foreach (var image in borderImages)
                {
                    image.color = initialColor;
                }
            }
            return;
        }
        // Actual character
        foreach(var image in borderImages)
        {
            image.color = character.characterColorLight;
        }
        arrowImage.enabled = true;
        arrowImage.color = character.characterColorLight;
        iconImage.sprite = character.chat_icon;
    }

    [ContextMenu("DoDim")]
    public void DoDim(TweenInfo tweenInfo)
    {
        tweenInfo.Complete();
        foreach (var canvasGroup in dimmingCanvasGroups)
        {
            tweenInfo.Start(canvasGroup.DOFade(dimAmount, tweenInfo.Time), false);
        }
        tweenInfo.Start(iconImage.DOColor(iconImage.color * dimColor, tweenInfo.Time), false);
        for(int i = 0; i < borderImages.Length; ++i)
        {
            var image = borderImages[i];
            tweenInfo.Start(image.DOColor(image.color * dimColor, tweenInfo.Time), false);
        }
        arrowImage.enabled = false;
        continueIndicator.Cleanup();
    }
}
