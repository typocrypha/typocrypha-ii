using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectCritBlock : BadgeEffect
{
    public const float baseCritChance = 0.20f;

    private int missCounter = 0;

    public override void Equip(Player player)
    {
        player.AddActiveAbilities(Caster.ActiveAbilities.CriticalBlock);
    }

    public override void Unequip(Player player)
    {
        player.RemoveActiveAbilities(Caster.ActiveAbilities.CriticalBlock);
    }

    public bool RollForCritical(Caster player)
    {
        float missComponent = missCounter * 0.1f + (player.Stats.Luck * 0.01f);
        if (UnityEngine.Random.Range(0, 1f) <= Mathf.Max(0, baseCritChance + missComponent + (player.Stats.Luck * 0.025f)))
        {
            missCounter = 0;
            return true;
        }
        else
        {
            missCounter++;
            return false;
        }
    }
}
