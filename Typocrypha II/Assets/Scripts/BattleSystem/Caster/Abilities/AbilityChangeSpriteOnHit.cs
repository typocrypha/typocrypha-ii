using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChangeSpriteOnHit : CasterAbility
{
    public SpellTag onHitTag;
    public Sprite sprite;

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
        if (!effect.tags.Contains(onHitTag) || target.BStatus == Caster.BattleStatus.SpiritMode)
        {
            return;
        }
        target.ui.onSpriteChanged?.Invoke(sprite);
    }
}
