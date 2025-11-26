using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIllyiaKillSteal : AIAllyRandomTimer
{
    [SerializeField] private List<Spell> followUpSpells;
    [SerializeField] private int threshold;
    [SerializeField] private double spiritModeIgnoreChance = 0.75;
    [SerializeField] private double targetedFollowUpChance = 0.6;

    private bool firstKillSteal = true;

    protected override void AddListeners()
    {
        RemoveListeners();
        Battlefield.instance.Player.OnAfterCastResolved += FollowUp;
    }

    protected override void DoAction()
    {
        CastAtRandomTarget(Battlefield.instance.Enemies, followUpSpells, true, TargetWeight);
    }

    private float TargetWeight(Caster target)
    {
        return target.Health <= threshold ? 150 : Math.Min(1, 100 - target.Health);
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
            if (enemy.BStatus == Caster.BattleStatus.SpiritMode && (firstKillSteal || RandomUtils.RandomU.instance.RollSuccess(spiritModeIgnoreChance)))
                continue;
            if(enemy.Health <= threshold && (firstKillSteal || RandomUtils.RandomU.instance.RollSuccess(targetedFollowUpChance)))
            {
                firstKillSteal = false;
                AllyBattleBoxManager.instance.ShakeBattleBox();
                InsertCast(enemy.FieldPos, RandomUtils.RandomU.instance.Choice(followUpSpells), true);
                return;
            }
        }
    }
}
