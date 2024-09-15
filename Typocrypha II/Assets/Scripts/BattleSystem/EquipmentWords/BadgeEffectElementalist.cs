using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectElementalist : BadgeEffect
{
    [SerializeField] private float multiplier = 0.25f;
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

    private void ApplyEffect(RootWordEffect effect, Caster caster, Caster target, Damage.DamageModifier mod)
    {
        var idSet = new HashSet<Typocrypha.KeyEffect.EffectType>();
        foreach(var kvp in Typocrypha.Keyboard.instance.allEffects)
        {
            if (!idSet.Contains(kvp.Value.EffectID))
            {
                idSet.Add(kvp.Value.EffectID);
            }
        }
        Debug.Log($"Count: {idSet.Count}");
        mod.damageMultiplier += (multiplier * idSet.Count);
    }
}
