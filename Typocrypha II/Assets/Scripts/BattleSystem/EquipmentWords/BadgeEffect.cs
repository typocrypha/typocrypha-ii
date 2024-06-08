using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BadgeEffect : ScriptableObject
{
    public abstract void Equip(Player player);
    public abstract void Unequip(Player player);
}
