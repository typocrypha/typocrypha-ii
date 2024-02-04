using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CasterStats
{
    public const int statMax = 20;
    public const int statMin = -statMax;
    public const int resourceMax = int.MaxValue;
    public const int resourceMin = -resourceMax;

    public float CastingSpeedMod
    {
        get => Damage.CompareStats(spd.Value, 0);
    }

    #region Resources
    [SerializeField] private Stat maxHP = new Stat(resourceMin, resourceMax);
    public int MaxHP { get => maxHP.Value; set => maxHP.Value = value; }
    [SerializeField] private Stat maxSP = new Stat(resourceMin, resourceMax);
    public int MaxSP { get => maxSP.Value; set => maxSP.Value = value; }
    [SerializeField] private Stat maxStagger = new Stat(resourceMin, resourceMax);
    public int MaxStagger { get => maxStagger.Value; set => maxStagger.Value = value; }
    [SerializeField] private float staggerTime = 0;
    public float StaggerTime { get { return staggerTime; } set { staggerTime = value; } }
    #endregion

    #region Stats
    [SerializeField] private Stat atk = new Stat(statMin, statMax);
    public int Atk { get => atk.Value; set => atk.Value = value; }
    [SerializeField] private Stat def = new Stat(statMin, statMax);
    public int Def { get => def.Value; set => def.Value = value; }
    [SerializeField] private Stat spd = new Stat(statMin, statMax);
    public int Spd { get => spd.Value; set => spd.Value = value; }
    [SerializeField] private Stat acc = new Stat(statMin, statMax);
    public int Acc { get => acc.Value; set => acc.Value = value; }
    [SerializeField] private Stat evade = new Stat(statMin, statMax);
    public int Evade { get => evade.Value; set => evade.Value = value; }
    [SerializeField] private Stat luck = new Stat(statMin, statMax);
    public int Luck { get => luck.Value; set => luck.Value = value; }
    #endregion

    public void AddInPlace(CasterStats other)
    {
        maxHP.AddInPlace(other.maxHP);
        maxSP.AddInPlace(other.maxSP);
        maxStagger.AddInPlace(other.maxStagger);
        staggerTime += other.staggerTime;
        atk.AddInPlace(other.atk);
        def.AddInPlace(other.def);
        spd.AddInPlace(other.spd);
        acc.AddInPlace(other.acc);
        evade.AddInPlace(other.evade);
        luck.AddInPlace(other.luck);
    }
    public void SubtractInPlace(CasterStats other)
    {
        maxHP.SubtractInPlace(other.maxHP);
        maxSP.SubtractInPlace(other.maxSP);
        maxStagger.SubtractInPlace(other.maxStagger);
        staggerTime -= other.staggerTime;
        atk.SubtractInPlace(other.atk);
        def.SubtractInPlace(other.def);
        spd.SubtractInPlace(other.spd);
        acc.SubtractInPlace(other.acc);
        evade.SubtractInPlace(other.evade);
        luck.SubtractInPlace(other.luck);
    }
    [System.Serializable]
    public class Stat
    {
        [SerializeField] private IntRange statRange;
        [SerializeField] private int overflow = 0;
        [SerializeField] private int _value;
        public int ValueUnclamped { get => _value + overflow; set => _value = statRange.Clamp(value, out overflow); }
        public int Value { get => _value; set => _value = statRange.Clamp(value, out overflow); }
        public Stat(int min, int max)
        {
            statRange = new IntRange(min, max);
        }
        public Stat(int min, int max, int value) : this(min, max)
        {
            Value = value;
        }
        public void AddInPlace(Stat other)
        {
            Value = ValueUnclamped + other.ValueUnclamped;
        }
        public void SubtractInPlace(Stat other)
        {
            Value = ValueUnclamped - other.ValueUnclamped;
        }
    }
}
