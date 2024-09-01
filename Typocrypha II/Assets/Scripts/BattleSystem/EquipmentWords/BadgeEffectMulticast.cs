using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectMulticast : BadgeEffect
{
    public int MaxWords => maxWords;
    [SerializeField] private int maxWords;
    public float WordMultiplier => wordMultiplier;
    [SerializeField] private float wordMultiplier;
}
