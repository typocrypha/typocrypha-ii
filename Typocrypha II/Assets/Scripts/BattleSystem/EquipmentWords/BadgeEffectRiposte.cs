using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectRiposte : BadgeEffect
{
    public const float baseRiposteChance = 0.15f;
    public const float fullCounterRiposteChance = 0.25f;

    [SerializeField] private SpellWord word;
    public int MaxUses => maxUses;
    [SerializeField] private int maxUses;

    public override void Equip(Player player)
    {
        player.OnCounterOther -= OnCounterOther;
        player.OnCounterOther += OnCounterOther;
    }

    public override void Unequip(Player player)
    {
        player.OnCounterOther -= OnCounterOther;
    }

    private void OnCounterOther(Caster caster, Caster countered, bool fullCounter)
    {
        SpellCooldownManager.instance.AddFixedUseWord(word, 1, maxUses, true);
    }
}
