using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TextPopup : DisplayPopup
{
    [SerializeField] private TextMeshProUGUI textUI;

    protected override Transform TargetTransform => textUI.transform;

    public Coroutine Play(string text, Color color, float time, Animation anim, IPool<TextPopup> pool)
    {
        textUI.color = color;
        textUI.text = text;
        textUI.transform.localScale = new Vector2(0, 1);
        void OnComplete()
        {
            pool.Release(this);
        }
        return StartCoroutine(DisplayCR(time, anim, OnComplete));
    }
}
