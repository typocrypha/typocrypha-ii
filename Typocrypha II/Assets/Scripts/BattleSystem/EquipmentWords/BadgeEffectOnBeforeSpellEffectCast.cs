using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BadgeEffectOnBeforeSpellEffectCast : BadgeEffect
{
    public override void Equip(Player player)
    {
        base.Equip(player);
        player.OnBeforeSpellEffectCast -= ApplyEffect;
        player.OnBeforeSpellEffectCast += ApplyEffect;
    }

    public override void Unequip(Player player)
    {
        base.Unequip(player);
        player.OnBeforeSpellEffectCast -= ApplyEffect;
    }

    protected abstract void ApplyEffect(RootWordEffect effect, Caster caster, Caster target, Damage.DamageModifier mod);
}
