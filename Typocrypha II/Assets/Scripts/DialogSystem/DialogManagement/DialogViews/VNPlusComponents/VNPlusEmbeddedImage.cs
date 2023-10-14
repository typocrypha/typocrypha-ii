using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class VNPlusEmbeddedImage : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public TweenInfo tweenInfo;
    [SerializeField] Image[] images;
    [SerializeField] private DialogContinueIndicator continueIndicator;

    [Header("Values")]
    public int messagesBeforeFade = 1;
    [SerializeField] Color dimColor;
    Tween tween;


    private void OnEnable()
    {
        transform.hasChanged = false;
    }

    //TODO: move to event in DialogViewMessage
    private void Update()
    {
        if (transform.hasChanged && tween == null)
        {
            if (transform.GetSiblingIndex() >= messagesBeforeFade) DoDim(tweenInfo);
            transform.hasChanged = false;
        }
    }

    public void DoDim(TweenInfo tweenInfo)
    {
        foreach (var image in images) tweenInfo.Start(tween = image.DOColor(image.color * dimColor, tweenInfo.Time), false);
        continueIndicator.Cleanup();
        Destroy(this, tweenInfo.Time);
    }
}
