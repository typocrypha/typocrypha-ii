using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectRegenerateOnWaveStart : BadgeEffectOnWaveStart
{
    [SerializeField] private SpellFxData animation;
    [Range(0,1)]
    [SerializeField] private float percentage;
    protected override Coroutine OnWaveStart(Caster player)
    {
        int hpMissing = player.Stats.MaxHP - player.Health;
        if (hpMissing <= 0)
            return null;
        int healAmount = Mathf.Min(hpMissing, Mathf.RoundToInt(player.Stats.MaxHP * percentage));
        if (healAmount <= 0)
            return null;
        player.Heal(healAmount);
        var results = new CastResults(player, player, -healAmount);
        results.AnimationData.Add(animation);
        var space = Battlefield.instance.GetSpaceScreenSpace(player.FieldPos);
        return SpellFxManager.instance.Play(results, space, space);
    }
}
