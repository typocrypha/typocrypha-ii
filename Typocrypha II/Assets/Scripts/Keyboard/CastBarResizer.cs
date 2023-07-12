using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastBarResizer : MonoBehaviour
{
    [SerializeField] private float spacePerLetter;
    [SerializeField] private float marginSpace;
    [SerializeField] private RectTransform target;

    public void Resize(int letters)
    {
        target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (spacePerLetter * letters) + marginSpace);
    }
}
