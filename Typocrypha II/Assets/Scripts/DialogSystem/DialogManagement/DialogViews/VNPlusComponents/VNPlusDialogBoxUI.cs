using DG.Tweening;
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
    [SerializeField] private CanvasGroup canvasGroup;

    public void Bind(CharacterData character)
    {
        foreach(var image in borderImages)
        {
            image.color = character.characterColorLight;
        }
        arrowImage.enabled = true;
        arrowImage.color = character.characterColorLight;
        iconImage.sprite = character.chat_icon;
        iconImage.color = Color.white;
        canvasGroup.alpha = 1;
    }

    public void DoDim(TweenInfo tweenInfo)
    {
        tweenInfo.Start(canvasGroup.DOFade(dimAmount, tweenInfo.Time));
        tweenInfo.Start(iconImage.DOColor(iconImage.color * dimColor, tweenInfo.Time), false);
        for(int i = 0; i < borderImages.Length; ++i)
        {
            var image = borderImages[i];
            tweenInfo.Start(image.DOColor(image.color * dimColor, tweenInfo.Time), false);
        }
        arrowImage.enabled = false;
    }
}
