using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectBurningTimeMultiplier : BadgeEffect
{
    public float Multiplier => multiplier;
    [Range(0, 10)]
    [SerializeField] private float multiplier = 0.5f;
}
