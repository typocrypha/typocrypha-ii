﻿using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TweenInfo
{
    public float Time => time;
    [SerializeField] private float time;
    [SerializeField] private bool useCustomEase;
    [SerializeField] private Ease ease;
    [SerializeField] private AnimationCurve customEase;

    private List<Tween> tweens;
    private List<Tween> Tweens => tweens = tweens ?? new List<Tween>();

    public TweenInfo(TweenInfo toCopy)
    {
        time = toCopy.time;
        useCustomEase = toCopy.useCustomEase;
        ease = toCopy.ease;
        customEase = new AnimationCurve(toCopy.customEase.keys);
    }

    public void Start(params Tween[] newTweens)
    {
        Complete();
        Tweens.AddRange(newTweens);
        foreach(var tween in Tweens)
        {
            if (useCustomEase)
            {
                tween.SetEase(customEase);
            }
            else
            {
                tween.SetEase(ease);
            }
        }
    }

    public void Complete()
    {
        foreach(var tween in Tweens)
        {
            tween.Complete();
        }
        Tweens.Clear();
    }

    public YieldInstruction WaitForCompletion()
    {
        return Tweens.Count > 0 ? Tweens[0].WaitForCompletion() : null;
    }
}
