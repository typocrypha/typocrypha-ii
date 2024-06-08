using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectRiposte : BadgeEffect
{
    public const float baseRiposteChance = 0.15f;
    public const float fullCounterRiposteChance = 0.25f;
    public override void Equip(Player player)
    {
        player.AddActiveAbilities(Caster.ActiveAbilities.Riposte);
    }

    public override void Unequip(Player player)
    {
        player.RemoveActiveAbilities(Caster.ActiveAbilities.Riposte);
    }

    public static bool RollForRiposte(Caster player, bool fullCounter)
    {
        if (fullCounter)
        {
            return UnityEngine.Random.Range(0, 1f) <= Mathf.Max(0, fullCounterRiposteChance + (player.Stats.Luck * 0.025f));
        }
        return UnityEngine.Random.Range(0, 1f) <= Mathf.Max(0, baseRiposteChance + (player.Stats.Luck * 0.015f));
    }
}
