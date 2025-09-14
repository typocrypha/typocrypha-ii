using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollowUpKillSteal : AIComponent
{
    [SerializeField] private List<Spell> followUpSpells;
    [SerializeField] private int threshold;

    protected override void AddListeners()
    {
        RemoveListeners();
        Battlefield.instance.Player.OnAfterCastResolved += FollowUp;
    }

    protected override void RemoveListeners()
    {
        Battlefield.instance.Player.OnAfterCastResolved -= FollowUp;
    }

    private void FollowUp(Spell s, Caster caster, bool hitTarget)
    {
        if (!hitTarget)
            return;
        foreach(var enemy in Battlefield.instance.Enemies)
        {
            if (enemy.IsDeadOrFled)
                continue;
            if (enemy.BStatus == Caster.BattleStatus.SpiritMode && RandomUtils.RandomU.instance.RandomDouble() < 0.75)
                continue;
            if(enemy.Health <= threshold)
            {
                AllyBattleBoxManager.instance.ShakeBattleBox();
                //InsertCast(enemy.FieldPos, followUpSpell, null, string.Empty);
                InsertCast(enemy.FieldPos, RandomUtils.RandomU.instance.Choice(followUpSpells), null);
                return;
            }
        }
        foreach (var enemy in Battlefield.instance.Enemies)
        {
            if (enemy.IsDeadOrFled || enemy.BStatus == Caster.BattleStatus.SpiritMode || enemy.FieldPos == Battlefield.instance.Player.TargetPos)
                continue;
            if (RandomUtils.RandomU.instance.RandomDouble() < 0.075)
            {
                AllyBattleBoxManager.instance.ShakeBattleBox();
                //InsertCast(enemy.FieldPos, followUpSpell, null, string.Empty);
                InsertCast(enemy.FieldPos, RandomUtils.RandomU.instance.Choice(followUpSpells), null);
                return;
            }
        }
    }
}
