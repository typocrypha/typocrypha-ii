using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Image imageUI;
    [SerializeField] private TweenInfo showHideTween;

    public Coroutine Play(Sprite sprite, Color color, float time, IPool<ImagePopup> pool)
    {
        imageUI.color = color;
        imageUI.sprite = sprite;
        imageUI.transform.localScale = Vector2.zero;
        imageUI.SetNativeSize();
        return StartCoroutine(PlayCR(time, pool));
    }

    private IEnumerator PlayCR(float time, IPool<ImagePopup> pool)
    {
        showHideTween.Start(group.DOFade(1, showHideTween.Time));
        showHideTween.Start(imageUI.transform.DOScale(1, showHideTween.Time), false);
        yield return showHideTween.WaitForCompletion();
        yield return new WaitForSeconds(time / Settings.UISpeed);
        showHideTween.Start(group.DOFade(0, showHideTween.Time));
        showHideTween.Start(imageUI.transform.DOScale(0, showHideTween.Time), false);
        yield return showHideTween.WaitForCompletion();
        pool.Release(this);
    }
}
