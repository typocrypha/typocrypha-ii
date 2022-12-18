using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleDialogBoxUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] frameImages;
    [SerializeField] private DialogContinueIndicator continueIndicator;
    [SerializeField] private Sprite genericIconSprite;

    private Color initialColor = Color.clear;

    public void Refresh()
    {
        if (initialColor == Color.clear)
        {
            initialColor = frameImages[0].color;
        }
        if (iconImage != null)
        {
            iconImage.color = Color.white;
        }
        continueIndicator.StopAllCoroutines();
    }

    public void Bind(CharacterData character)
    {
        Refresh();
        // Generic Character functionality
        if (character == null)
        {
            foreach (var image in frameImages)
            {
                image.color = initialColor;
            }
            if (iconImage != null)
            {
                iconImage.sprite = genericIconSprite;
            }
            return;
        }
        // Actual character
        foreach (var image in frameImages)
        {
            image.color = character.characterColorLight;
        }
        if(iconImage != null)
        {
            iconImage.sprite = character.chat_icon;
        }
    }
}
