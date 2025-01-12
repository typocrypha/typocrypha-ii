using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiledBackgroundScroller : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private float scrollSpeed = 1.0f;

    private RectTransform imageTransform;

    private void Start()
    {
        imageTransform = (RectTransform)image.transform;

        var target = new Vector2(imageTransform.anchoredPosition.x + image.sprite.rect.width,
                                   imageTransform.anchoredPosition.y + image.sprite.rect.height);
        imageTransform.DOAnchorPos(target, scrollSpeed)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}
