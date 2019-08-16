using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellWord : ScriptableObject
{   
    public string internalName;
    public string description;
    public float cost;
    public abstract SpellWord Clone();
}
