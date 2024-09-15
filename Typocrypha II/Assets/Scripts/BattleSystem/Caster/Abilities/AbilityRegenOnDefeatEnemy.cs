using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRegenOnDefeatEnemy : CasterAbility
{
    [SerializeField] [Range(0, 1)] private float percentage;
    private Caster self;
    public override void AddTo(Caster caster)
    {
        self = caster;
        caster.OnAfterSpellEffectCast += OnAfterSpellEffectCast;
        caster.OnBeforeSpellEffectCast += OnBeforeSpellEffectCast;
    }

    public override void RemoveFrom(Caster caster)
    {
        self = null;
        caster.OnAfterSpellEffectCast -= OnAfterSpellEffectCast;
        caster.OnBeforeSpellEffectCast -= OnBeforeSpellEffectCast;
    }

    private void OnBeforeSpellEffectCast(RootWordEffect effect, Caster caster, Caster target, Damage.DamageModifier mod)
    {
        target.OnSpiritMode += OnTargetSpiritMode;
    }

    private void OnTargetSpiritMode()
    {
        if(self != null && self.Health < self.Stats.MaxHP)
        {
            int amountHealed = Math.Min(Mathf.RoundToInt(self.Stats.MaxHP * percentage), self.Stats.MaxHP - self.Health);
            self.Heal(amountHealed);
            SpellFxManager.instance.PlayDamageNumber(-amountHealed, Battlefield.instance.GetSpaceScreenSpace(self.FieldPos));
        }
    }

    private void OnAfterSpellEffectCast(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        target.OnSpiritMode -= OnTargetSpiritMode;
    }
}
