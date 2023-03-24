using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private TweenInfo showHideTween;

    public Coroutine Play(string text, Color color, float time, IPool<TextPopup> pool)
    {
        textUI.color = color;
        textUI.text = text;
        textUI.transform.localScale = new Vector2(0, 1);
        return StartCoroutine(PlayCR(time, pool));
    }

    private IEnumerator PlayCR(float time, IPool<TextPopup> pool)
    {
        showHideTween.Start(group.DOFade(1, showHideTween.Time));
        showHideTween.Start(textUI.transform.DOScale(1, showHideTween.Time), false);
        yield return showHideTween.WaitForCompletion();
        yield return new WaitForSeconds(time / Settings.UISpeed);
        showHideTween.Start(group.DOFade(0, showHideTween.Time));
        showHideTween.Start(textUI.transform.DOScale(0, showHideTween.Time), false);
        yield return showHideTween.WaitForCompletion();
        pool.Release(this);
    }
}
