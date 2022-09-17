using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class DialogContinueIndicator : MonoBehaviour
{
    [SerializeField] private Image continueIndicator;
    [SerializeField] private float moveXDistance = 5;
    [SerializeField] private float moveXDuration = 0.25f;
    [SerializeField] private float indicatorDelay = 0.0f;
    private WaitForSeconds indicatorDelaySeconds;

    [SerializeField] private DialogBox dialogBox;


    private Vector3 originalIndicatorPosition = Vector3.zero;

    private Coroutine activeCoroutine = null;
    private object activeTween = null;

    private void Start()
    {
        indicatorDelaySeconds = new WaitForSeconds(indicatorDelay);
        originalIndicatorPosition = continueIndicator.rectTransform.localPosition;
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
        activeTween = continueIndicator.rectTransform.DOLocalMoveX(originalIndicatorPosition.x + moveXDistance, moveXDuration, false)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .id;
    }

    private void StopAnimation()
    {
        if (activeTween != null)
        {
            DOTween.Kill(activeTween);
            activeTween = null;
        }
        continueIndicator.rectTransform.localPosition = originalIndicatorPosition;
        continueIndicator.enabled = false;
    }
}
