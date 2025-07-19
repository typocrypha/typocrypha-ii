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
        SlamIn,
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
            TargetTransform.DOMoveY(TargetTransform.position.y + 65, modTime).SetEase(Ease.OutQuint);
            group.DOFade(0, modTime * 0.45f).SetEase(Ease.InCubic).SetDelay(modTime * 0.55f);
            yield return new WaitForSeconds(modTime);
        }
        else if(anim == Animation.ShowHide)
        {
            yield return new WaitForSeconds(time / Settings.UISpeed);
            showHideTween.Start(group.DOFade(0, showHideTween.Time));
            showHideTween.Start(TargetTransform.transform.DOScale(0, showHideTween.Time), false);
            yield return showHideTween.WaitForCompletion();
        }
        else if(anim == Animation.SlamIn)
        {
            float modTime = (time / Settings.UISpeed);
            TargetTransform.DOPunchScale(new Vector3(0.25f, 0.25f), modTime * 0.5f, 10);
            group.DOFade(0, modTime * 0.45f).SetEase(Ease.InCubic).SetDelay(modTime * 0.55f);
            yield return new WaitForSeconds(modTime);
        }
        onComplete?.Invoke();
    }
}
