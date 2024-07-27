using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectAddAbilityOnCounter : BadgeEffect
{
    [SubSO("Ability")]
    [SerializeField] private CasterAbility ability;

    public override void Equip(Player player)
    {
        player.OnCounterOther -= OnCounter;
        player.OnCounterOther += OnCounter;
    }

    public override void Unequip(Player player)
    {
        player.OnCounterOther += OnCounter;
    }

    private void OnCounter(Caster caster, Caster countered, bool fullCounter)
    {
        if (!fullCounter)
            return;
        ability.AddTo(countered);
    }
}
