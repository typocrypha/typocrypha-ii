using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFurious : CasterAbility
{
    private int addedSpeed = 0;
    public override void AddTo(Caster caster)
    {
        addedSpeed = 0;
        caster.OnAfterHitResolved -= OnAfterHitResolved;
        caster.OnAfterHitResolved += OnAfterHitResolved;
    }

    private void OnAfterHitResolved(RootWordEffect effect, Caster caster, Caster self, RootCastData spellData, CastResults data)
    {
        if (caster == self || !data.WillDealDamage)
            return;
        AddSpeed(self, 1);
    }

    public override void RemoveFrom(Caster caster)
    {
        RemoveAddedSpeed(caster);
        caster.OnAfterHitResolved -= OnAfterHitResolved;
    }

    private void AddSpeed(Caster self, int amount)
    {
        self.Stats.Spd += amount;
        addedSpeed += amount;
        Debug.Log($"CurrSpeed+: {self.Stats.Spd}");
    }

    private void RemoveAddedSpeed(Caster self)
    {
        self.Stats.Spd -= addedSpeed;
        Debug.Log($"CurrSpeed-: {self.Stats.Spd}");
        addedSpeed = 0;
    }
}
