using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class VNPlusEmbeddedImage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image[] images;
    [SerializeField] private DialogContinueIndicator continueIndicator;

    [Header("Values")]
    public int messagesBeforeFade = 1;
    [SerializeField] float tweenDuration = 0.55f;
    [SerializeField] Color dimColor;
    Tween tween;

    [ContextMenu("DoDim")]

    private void OnEnable()
    {
        transform.hasChanged = false;
    }

    private void Update()
    {
        if (transform.hasChanged && tween == null)
        {
            if (transform.GetSiblingIndex() >= messagesBeforeFade) DoDim();
            transform.hasChanged = false;
        }
    }

    public void DoDim()
    {
        foreach (var image in images) tween = image.DOColor(image.color * dimColor, tweenDuration);
        continueIndicator.Cleanup();
        Destroy(this, tweenDuration);
    }
}
