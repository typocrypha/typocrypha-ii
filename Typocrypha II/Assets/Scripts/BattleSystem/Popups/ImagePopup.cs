using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePopup : DisplayPopup
{
    [SerializeField] private Image imageUI;

    protected override Transform TargetTransform => imageUI.transform;

    public Coroutine Play(Sprite sprite, Color color, float time, Animation anim, IPool<ImagePopup> pool)
    {
        imageUI.color = color;
        imageUI.sprite = sprite;
        imageUI.transform.localScale = Vector2.zero;
        imageUI.SetNativeSize();
        void OnComplete()
        {
            pool.Release(this);
        }
        return StartCoroutine(DisplayCR(time, anim, OnComplete));
    }
}
