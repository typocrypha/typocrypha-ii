using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleFriendshipFrog : Rule
{
    private const string ruleName = "Friendship Frog";
    public override string DisplayName => ruleName;

    public override bool ApplyToEffect(RootWordEffect effect, Caster caster, Caster target)
    {
        if (caster.HasTag("Frog") || GetEnemyCount() < 2)
        {
            return false;
        }
        effect.tags.Add(Lookup.GetSpellTag("Frogified"));
        return true;
    }

    public static bool ProtectsCaster(Caster caster)
    {
        return ActiveRule != null && ActiveRule.DisplayName == ruleName && caster.HasTag("Frog") && GetEnemyCount() > 1;
    }

    private static int GetEnemyCount()
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
