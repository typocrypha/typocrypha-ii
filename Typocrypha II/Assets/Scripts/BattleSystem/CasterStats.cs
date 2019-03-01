﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CasterStats
{
    public static readonly IntRange statRange = new IntRange(-10, 10);

    #region Resource Maxes
    public int maxHP = 100;
    public int maxStagger = 1;
    #endregion

    #region Stats
    [SerializeField] private float staggerTime = 5f;
    public float StaggerTime { get { return staggerTime; } set { staggerTime = value > 0 ? value : 0; } }
    [SerializeField] private int atk;
    public int Atk { get { return atk; } set { atk = statRange.Clamp(value); } }
    [SerializeField] private int def;
    public int Def { get { return def; } set { def = statRange.Clamp(value); } }
    [SerializeField] private int spd;
    public int Spd { get { return spd; } set { spd = statRange.Clamp(value); } }
    [SerializeField] private int acc;
    public int Acc { get { return acc; } set { acc = statRange.Clamp(value); } }
    [SerializeField] private int evade;
    public int Evade { get { return evade; } set { evade = statRange.Clamp(value); } }
    #endregion
}
