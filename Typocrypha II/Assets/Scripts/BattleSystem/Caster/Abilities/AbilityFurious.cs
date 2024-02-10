using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFurious : CasterAbility
{
    private int addedSpeed = 0;
    public override void AddTo(Caster caster)
    {
        RemoveFrom(caster);
        addedSpeed = 0;
        caster.OnCounter += OnCounter;
        caster.OnAfterCastResolved += AfterCastResolved;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.OnCounter -= OnCounter;
        caster.OnAfterCastResolved -= AfterCastResolved;
    }

    private void OnCounter(Caster self, bool fullCounter)
    {
        int speedBoost = fullCounter ? 2 : 2;
        addedSpeed += speedBoost;
        self.Stats.Spd += speedBoost;
        if (fullCounter)
        {
            self.Stagger -= 1;
        }
    }

    private void AfterCastResolved(Spell spell, Caster self)
    {
        if (spell.Countered)
            return;
        self.Stats.Spd -= addedSpeed;
        addedSpeed = 0;
    }

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target) { }

    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults) { }
}
