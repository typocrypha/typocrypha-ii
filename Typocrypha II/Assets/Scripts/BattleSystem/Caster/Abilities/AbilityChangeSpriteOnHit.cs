using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChangeSpriteOnHit : CasterAbility
{
    public SpellTag onHitTag;
    public Sprite sprite;

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {

    }

    public override void OnBeforeHitApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if (!effect.tags.Contains(onHitTag) || target.BStatus == Caster.BattleStatus.SpiritMode)
        {
            return;
        }
        target.ui.onSpriteChanged?.Invoke(sprite);
    }
}
