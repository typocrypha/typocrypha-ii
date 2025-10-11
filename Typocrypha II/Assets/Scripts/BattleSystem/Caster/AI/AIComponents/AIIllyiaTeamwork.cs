using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIllyiaTeamwork : AIAllyRandomTimer
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

    protected override void DoAction()
    {
        foreach (var enemy in Battlefield.instance.Enemies)
        {
            if (enemy.IsDeadOrFled)
                continue;
            if (enemy.BStatus == Caster.BattleStatus.SpiritMode)
                continue;
            if (enemy.Health <= threshold && enemy.ChargeNormalized >= 0.9f)
            {
                AllyBattleBoxManager.instance.ShakeBattleBox();
                QueueCast(enemy.FieldPos, RandomUtils.RandomU.instance.Choice(followUpSpells), null);
                return;
            }
        }
        if (RandomUtils.RandomU.instance.RollSuccess(0.2))
        {
            CastAtRandomTarget(Battlefield.instance.Enemies, followUpSpells);
        }
    }

    private void FollowUp(Spell s, Caster caster, bool hitTarget)
    {
        if (!hitTarget || !RandomUtils.RandomU.instance.RollSuccess(0.1))
            return;
        CastAtRandomTarget(Battlefield.instance.Enemies, followUpSpells);
    }
}
