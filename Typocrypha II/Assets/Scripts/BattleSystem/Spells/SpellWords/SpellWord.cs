using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellWord : ScriptableObject
{
    public enum Category
    {
        Attack,
        Defense,
        Heal,
        Status,
        Modifier,
        Special,
    }
    public string Key => internalName.ToLower();
    public string internalName;
    public string description;
    public float cost;
    public int cooldown = 3;
    public Category category;
    public SpellWord synonymOf;

    public static bool CompareKeys(SpellWord w1, SpellWord w2) => w1.Key == w2.Key;

    public abstract SpellWord Clone();
}
