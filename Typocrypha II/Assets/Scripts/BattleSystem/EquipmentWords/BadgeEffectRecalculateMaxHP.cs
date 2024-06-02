using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectRecalculateMaxHP : BadgeEffect
{
    public override void Equip(Player player)
    {
        player.RecalculateMaxHP();
    }

    public override void Unequip(Player player)
    {
        player.RecalculateMaxHP();
    }
}
