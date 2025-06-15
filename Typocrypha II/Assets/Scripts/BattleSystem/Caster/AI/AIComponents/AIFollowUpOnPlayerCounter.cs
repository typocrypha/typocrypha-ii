using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollowUpOnPlayerCounter : AIComponent
{
    [SerializeField] private Spell followUpSpell;
    [SerializeField] private List<Caster> targetFilter;

    protected override void AddListeners()
    {
        RemoveListeners();
        Battlefield.instance.Player.OnCounterOther += FollowUp;
    }

    protected override void RemoveListeners()
    {
        Battlefield.instance.Player.OnCounterOther -= FollowUp;
    }

    private void FollowUp(Caster counterCaster, Caster countered, bool fullCounter)
    {
        if (!fullCounter || caster.IsDeadOrFled || !PassesFilter(countered))
            return;
        AllyBattleBoxManager.instance.ShakeBattleBox();
        InsertCast(countered.FieldPos, followUpSpell, null);
    }

    private bool PassesFilter(Caster countered)
    {
        if (targetFilter.Count <= 0)
        {
            return true;
        }
        foreach(var caster in targetFilter)
        {
            if (caster.DisplayName == countered.DisplayName)
                return true;
        }
        return false;
    }
}
