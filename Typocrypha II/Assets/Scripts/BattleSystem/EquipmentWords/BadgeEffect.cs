using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BadgeEffect : ScriptableObject
{
    public virtual void Equip(Player player) { }
    public virtual void Unequip(Player player) { }
}
