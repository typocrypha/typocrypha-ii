using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectRecalculateMaxHP : BadgeEffect
{
    public override void Equip(Caster player)
    {
        player.RecalculateMaxHP();
    }

    public override void Unequip(Caster player)
    {
        player.RecalculateMaxHP();
    }
}
