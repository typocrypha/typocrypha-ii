using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BadgeEffect : ScriptableObject
{
    public abstract void Equip(Caster player);
    public abstract void Unequip(Caster player);
}
