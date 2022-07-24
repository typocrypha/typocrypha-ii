using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class TweenInfo
{
    public float Time => time;
    [SerializeField] private float time;
    [SerializeField] private bool useCustomEase;
    [SerializeField] private Ease ease;
    [SerializeField] private AnimationCurve customEase;

    public Tween Tween => currTween;
    private Tween currTween;

    public TweenInfo(TweenInfo toCopy)
    {
        time = toCopy.time;
        useCustomEase = toCopy.useCustomEase;
        ease = toCopy.ease;
        customEase = new AnimationCurve(toCopy.customEase.keys);
    }

    public void Start(Tween tween)
    {
        Complete();
        currTween = tween;
        if (useCustomEase)
        {
            currTween.SetEase(customEase);
        }
        else 
        {
            currTween.SetEase(ease);
        }
    }

    public void Complete()
    {
        currTween?.Complete();
        currTween = null;
    }

    public YieldInstruction WaitForCompletion()
    {
        return currTween?.WaitForCompletion();
    }
}
