using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DisplayPopup : MonoBehaviour 
{
    public enum Animation
    {
        ShowHide,
        FloatUp,
    }
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TweenInfo showHideTween;

    protected abstract Transform TargetTransform { get; }

    protected IEnumerator DisplayCR(float time, Animation anim, System.Action onComplete)
    {
        TargetTransform.localPosition = Vector3.zero;
        showHideTween.Start(group.DOFade(1, showHideTween.Time));
        showHideTween.Start(TargetTransform.DOScale(1, showHideTween.Time), false);
        yield return showHideTween.WaitForCompletion();
        if (anim == Animation.FloatUp)
        {
            float modTime = (time / Settings.UISpeed);
            TargetTransform.DOMoveY(TargetTransform.position.y + 50, modTime).SetEase(Ease.OutQuint);
            group.DOFade(0, modTime).SetEase(Ease.InCubic);
            yield return new WaitForSeconds(modTime);
        }
        else
        {
            yield return new WaitForSeconds(time / Settings.UISpeed);
            showHideTween.Start(group.DOFade(0, showHideTween.Time));
            showHideTween.Start(TargetTransform.transform.DOScale(0, showHideTween.Time), false);
            yield return showHideTween.WaitForCompletion();
        }
        onComplete?.Invoke();
    }
}
