using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ClarkeShopTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image clarkeImage;
    [SerializeField] private RectTransform clarkeRT;
    [SerializeField] private RectTransform backgroundRT;

    private void Start()
    {
        SetHidden();
    }


    [ContextMenu("Preview Intro Animation", false)]
    public void PlayIntroAnimation(Action onCoverComplete = null)
    {
        SetHidden();
        var seq = DOTween.Sequence();
        // Begin cover
        seq.Append(canvasGroup.DOFade(1f, 0.25f))
            .Join(clarkeRT.DOScale(1f, 0.25f))
            .Join(clarkeImage.DOFade(1f, 0.25f))
            .Join(clarkeRT.DOPunchRotation(new Vector3(0f, 0f, 180f), 0.25f))
            .Insert(0.15f, backgroundRT.DOSizeDelta(new Vector2(3600f, 3600f), 0.5f))
            // Cover complete
            .AppendCallback(() => onCoverComplete?.Invoke())
            // Begin fade out
            .Append(canvasGroup.DOFade(0f, 0.25f))
            .Join(clarkeRT.DOScale(0f, 0.25f))
            .Join(clarkeRT.DOPunchRotation(new Vector3(0f, 0f, -180f), 0.25f))
            .Join(backgroundRT.DOSizeDelta(new Vector2(0f, 0f), 0.25f))
            // Fade out complete
            .AppendCallback(SetHidden);
    }

    [ContextMenu("Preview Exit Animation", false)]
    public void PlayExitAnimation()
    {

    }

    private void SetHidden()
    {
        canvasGroup.alpha = 1f;
        clarkeImage.color = new Color(1f, 1f, 1f, 0f);
        clarkeRT.localScale = Vector3.zero;
        clarkeRT.eulerAngles = Vector3.zero;
        backgroundRT.sizeDelta = Vector2.zero;
    }
}
