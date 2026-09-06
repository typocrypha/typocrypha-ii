using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollowUpOnPlayerCounter : AIComponent
{
    [SerializeField] private Spell followUpSpell;
    [SerializeField] private List<Caster> targetFilter;
    [SerializeField] private bool canCounter = true;

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
        if (!fullCounter || caster.IsInactive|| !PassesFilter(countered))
            return;
        AllyBattleBoxManager.instance.ShakeBattleBox();
        InsertCast(countered.FieldPos, followUpSpell, canCounter, null, string.Empty);
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
