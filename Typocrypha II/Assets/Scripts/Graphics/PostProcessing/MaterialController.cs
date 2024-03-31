//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialController : MonoBehaviour
{
    public Material material;
    public string propertyName;
    public float initialValue;
    public float finalValue;
    public float duration;
    public Ease ease;

    private float propertyValue;
    private Tween tween;

    public void Start()
    {
        var defaultAutoPlay = DOTween.defaultAutoPlay;
        DOTween.defaultAutoPlay = AutoPlay.None;
        tween = DOTween.To(() => propertyValue, v => propertyValue = v, finalValue, duration).From(initialValue).SetEase(ease)
            .OnUpdate(() => material.SetFloat(propertyName, propertyValue)).SetAutoKill(false);
        DOTween.defaultAutoPlay = defaultAutoPlay;
    }

    public void Play() => tween.Restart();
}
