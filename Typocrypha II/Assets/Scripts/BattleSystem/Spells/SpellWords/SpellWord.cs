using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellWord : ScriptableObject
{
    public string Key => internalName.ToLower();
    public string internalName;
    public string description;
    public float cost;
    public int cooldown = 3;
    public abstract SpellWord Clone();
}
