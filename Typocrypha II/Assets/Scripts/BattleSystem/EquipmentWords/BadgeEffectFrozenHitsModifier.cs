using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectFrozenHitsModifier : BadgeEffect
{
    public int Modifier => modifier;
    [SerializeField] private int modifier = -1;
}
