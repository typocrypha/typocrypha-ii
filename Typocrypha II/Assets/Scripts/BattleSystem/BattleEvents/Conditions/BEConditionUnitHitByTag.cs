using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionUnitHitByTag : BEConditionUnitHit
{
    [SerializeField] private SpellTag spellTag;
    protected override bool CheckHit(RootWordEffect effect, Caster caster, Caster self, RootCastData spellData, CastResults data)
    {
        return effect.tags.Contains(spellTag);
    }
}
