using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleFriendshipFrog : Rule
{
    public override bool ApplyToEffect(RootWordEffect effect, Caster caster, Caster target)
    {
        if (caster.HasTag("Frog") || GetEnemyCount() < 2)
        {
            return false;
        }
        effect.tags.Add(SpellLookup.instance.GetSpellTag("Frogified"));
        return true;
    }

    private int GetEnemyCount()
    {
        int enemyCount = 0;
        foreach (var caster in Battlefield.instance.Casters)
        {
            if (caster.CasterState == Caster.State.Hostile && caster.BStatus == Caster.BattleStatus.Normal)
            {
                enemyCount++;
            }
        }
        return enemyCount;
    }
}
