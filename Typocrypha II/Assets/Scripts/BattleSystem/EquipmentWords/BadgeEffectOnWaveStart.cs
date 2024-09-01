using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BadgeEffectOnWaveStart : BadgeEffect
{
    public override void Equip(Player player)
    {
        base.Equip(player);
        player.OnWaveStart -= OnWaveStart;
        player.OnWaveStart += OnWaveStart;
    }

    public override void Unequip(Player player)
    {
        base.Unequip(player);
        player.OnWaveStart -= OnWaveStart;
    }

    protected abstract Coroutine OnWaveStart(Caster player);
}
