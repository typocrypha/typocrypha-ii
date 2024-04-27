//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialController : MonoBehaviour
{
    [Header("Material")]
    public Material materialAsset;
    public bool cloneMaterialOnStart = true;
    [Header("Tween")]
    public string propertyName;
    public float initialValue;
    public float finalValue;
    public float duration;
    public Ease ease = Ease.Linear;
    public bool shouldResetProperty = false;
    public float resetValue = 0;

    private float propertyValue
    {
        get => material.GetFloat(propertyName); set => material.SetFloat(propertyName, value);
    }
    private Tween tween;
    private Material material;

    void Awake()
    {
        material = cloneMaterialOnStart ? new Material(materialAsset) : materialAsset;
        var processing = Camera.main.gameObject.AddComponent<PostProcess>();
        processing.mat = material;
    }

    public Tween GetTween(bool forceInit = false)
    {
        if (!forceInit && tween.IsActive()) return tween;
        var defaultAutoPlay = DOTween.defaultAutoPlay;
        DOTween.defaultAutoPlay = AutoPlay.None;
        tween = DOTween.To(() => propertyValue, v => propertyValue = v, finalValue, duration)
            .From(initialValue, false)
            .SetEase(ease)
            .SetAutoKill(false)
            .SetRecyclable(true);
        if (shouldResetProperty) tween.OnComplete(() => propertyValue = resetValue);
        DOTween.defaultAutoPlay = defaultAutoPlay;
        return tween;
    }

    public Tween Play()
    {
        if (!tween.IsActive())
        {
            tween = GetTween();
        }
        tween.Restart();
        return tween;
    }
}
