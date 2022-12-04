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

    [SerializeField] private DialogBox dialogBox;


    private Vector3 originalIndicatorPosition = Vector3.zero;
    private Vector3 originalIndicatorScale = Vector3.zero;

    private Coroutine activeCoroutine = null;
    private object activeMoveTween = null;
    private object activeScaleTween = null;

    private void Start()
    {
        indicatorDelaySeconds = new WaitForSeconds(indicatorDelay);
        originalIndicatorPosition = continueIndicator.rectTransform.localPosition;
        originalIndicatorScale = continueIndicator.rectTransform.localScale;
    }

    private void OnEnable()
    {
        continueIndicator.enabled = false;
        activeCoroutine = StartCoroutine(ActivateContinueIndicator());
    }

    private void OnDisable()
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
        yield return null;

        continueIndicator.enabled = false;

        yield return new WaitUntil(() => dialogBox.IsDone);
        yield return indicatorDelaySeconds;

        AnimateIndicator();

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        
        StopAnimation();
    }

    private void AnimateIndicator()
    {
        continueIndicator.enabled = true;
        originalIndicatorPosition = continueIndicator.rectTransform.localPosition;
        activeMoveTween = continueIndicator.rectTransform.DOLocalMoveY(originalIndicatorPosition.y - moveYDistance, animationDuration, false)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .id;
        activeScaleTween = continueIndicator.rectTransform.localScale;
        activeScaleTween = continueIndicator.rectTransform.DOScaleY(scaleYAmount, animationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .id;
    }

    private void StopAnimation()
    {
        if (activeMoveTween != null)
        {
            DOTween.Kill(activeMoveTween);
            activeMoveTween = null;
        }
        if (activeScaleTween != null)
        {
            DOTween.Kill(activeScaleTween);
            activeScaleTween = null;
        }
        continueIndicator.rectTransform.localPosition = originalIndicatorPosition;
        continueIndicator.rectTransform.localScale = originalIndicatorScale;
        continueIndicator.enabled = false;
    }
}
