using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Allows sliced images to be used as horizontal fills
[ExecuteInEditMode]
public class FilledSlicedImage : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private float minWidth;
    [SerializeField] private float maxWidth;
    
    [Range(0, 1)]
    [SerializeField] private float fillAmount;
    public float FillAmount
    {
        get => fillAmount;
        set
        {
            fillAmount = Mathf.Clamp01(value);
            UpdateWidth();
        }
    }

    private void UpdateWidth()
    {
        float width = ((maxWidth - minWidth) * FillAmount) + minWidth;
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Update()
    {
        if (image == null)
            return;
        UpdateWidth();
    }
}
