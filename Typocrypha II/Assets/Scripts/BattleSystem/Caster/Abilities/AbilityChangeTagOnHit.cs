﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChangeTagOnHit : CasterAbility
{
    public SpellTag onHitTag;
    public CasterTag removeTag;
    public CasterTag addTag;

    public override void OnBeforeSpellEffectResolved(RootWordEffect effect, Caster caster, Caster target)
    {

    }

    public override void OnHit(RootWordEffect effect, Caster caster, Caster target, CastResults castResults)
    {
        if (!effect.tags.Contains(onHitTag))
        {
            return;
        }
        target.RemoveTag(removeTag);
        target.AddTag(addTag);
    }
}
