using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionUnitHitWithReaction : BEConditionUnitHit
{
    [SerializeField] private Reaction reaction;
    protected override bool CheckHit(RootWordEffect effect, Caster caster, Caster self, RootCastData spellData, CastResults data)
    {
        return data.Effectiveness == reaction;
    }
}
