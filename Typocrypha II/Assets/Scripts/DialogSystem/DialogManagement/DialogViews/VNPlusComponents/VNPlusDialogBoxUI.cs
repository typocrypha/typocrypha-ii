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
    [SerializeField] private CanvasGroup canvasGroup;
    public void Bind(CharacterData character)
    {
        foreach(var image in borderImages)
        {
            image.color = character.characterColorLight;
        }
        iconImage.sprite = character.chat_icon;
    }

    public void DoDim(TweenInfo tweenInfo)
    {
        var allTweens = new Tween[borderImages.Length + nonBorderComponents];
        allTweens[0] = canvasGroup.DOFade(dimAmount, tweenInfo.Time);
        allTweens[1] = iconImage.DOColor(iconImage.color * dimColor, tweenInfo.Time);
        for(int i = 0; i < borderImages.Length; ++i)
        {
            var image = borderImages[i];
            allTweens[i + nonBorderComponents] = image.DOColor(image.color * dimColor, tweenInfo.Time);
        }
        tweenInfo.Start(allTweens);
    }
}
