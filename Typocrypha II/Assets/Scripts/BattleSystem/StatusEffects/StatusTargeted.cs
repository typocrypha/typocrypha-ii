using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusTargeted : StatusRemoveAfterHitOrCast
{
    protected override void Initialize()
    {
        base.Initialize();
        affected.OnBeforeHitResolved += ApplyStatus;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        affected.OnBeforeHitResolved -= ApplyStatus;
    }

    private void ApplyStatus(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        if (data.Mod == Damage.SpecialModifier.CritBlock || data.Effectiveness == Reaction.Block)
            return;
        Damage.StandardSpecialMod(data, effect, caster, target, Damage.SpecialModifier.Critical);
    }
}
