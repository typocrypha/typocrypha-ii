using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class DialogContinueIndicator : MonoBehaviour
{
    [SerializeField] private Image continueIndicator;
    [SerializeField] private float moveYDistance = 5;
    [SerializeField] private float scaleYAmount = 0.75f;
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float indicatorDelay = 0.0f;
    private WaitForSeconds indicatorDelaySeconds;

    private Vector3 originalIndicatorPosition = Vector3.zero;
    private Vector3 originalIndicatorScale = Vector3.zero;

    private Coroutine activeCoroutine = null;
    private Tween activeMoveTween = null;
    private Tween activeScaleTween = null;

    public void Activate()
    {
        Cleanup();
        activeCoroutine = StartCoroutine(ActivateContinueIndicator());
    }

    private void Awake()
    {
        indicatorDelaySeconds = new WaitForSeconds(indicatorDelay);
        originalIndicatorPosition = continueIndicator.rectTransform.localPosition;
        originalIndicatorScale = continueIndicator.rectTransform.localScale;
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    public void Cleanup()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
        StopAnimation();
    }

    IEnumerator ActivateContinueIndicator()
    {
        yield return indicatorDelaySeconds;

        AnimateIndicator();

        yield return new WaitUntil(() => !DialogManager.instance.PH.Pause && Input.GetKeyDown(KeyCode.Space));
        
        StopAnimation();
    }

    private void AnimateIndicator()
    {
        TurnIndicatorOn();
        originalIndicatorPosition = continueIndicator.rectTransform.localPosition;
        activeMoveTween = continueIndicator.rectTransform.DOLocalMoveY(originalIndicatorPosition.y - moveYDistance, animationDuration, false)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        originalIndicatorScale = continueIndicator.rectTransform.localScale;
        activeScaleTween = continueIndicator.rectTransform.DOScaleY(scaleYAmount, animationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopAnimation()
    {
        if (activeMoveTween != null)
        {
            activeMoveTween.Kill(false);
            activeMoveTween = null;
        }
        if (activeScaleTween != null)
        {
            activeScaleTween.Kill(false);
            activeScaleTween = null;
        }
        continueIndicator.rectTransform.localPosition = originalIndicatorPosition;
        continueIndicator.rectTransform.localScale = originalIndicatorScale;
        TurnIndicatorOff();
    }

    private void TurnIndicatorOff()
    {
        continueIndicator.color = Color.clear;
    }

    private void TurnIndicatorOn()
    {
        continueIndicator.color = Color.white;
    }
}
