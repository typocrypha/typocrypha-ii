using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDestroyIfHitByTag : CasterAbility
{
    public SpellTag tag;
    public Spell castOnDestroy;
    public string message;

    public override void AddTo(Caster caster)
    {
        caster.OnBeforeHitResolved += OnBeforeHitResolved;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.OnBeforeHitResolved -= OnBeforeHitResolved;
    }

    public void OnBeforeHitResolved(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if (!effect.tags.Contains(tag) || !castResults.WillDealDamage || target.BStatus != Caster.BattleStatus.Normal)
        {
            return;
        }
        SpellManager.instance.LogInterruptCast(castOnDestroy, target, target.FieldPos, string.IsNullOrWhiteSpace(message) ? null : target.DisplayName + " " + message);
        castResults.Damage = target.Health * 2;
    }
}
