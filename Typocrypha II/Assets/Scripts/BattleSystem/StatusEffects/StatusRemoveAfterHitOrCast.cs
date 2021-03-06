﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRemoveAfterHitOrCast : StatusEffect
{
    public bool removeFromHits = true;
    public int hitsToRemove = 1;
    public bool removeFromCasts = true;
    public int castsToRemove = 1;
    bool firstCastDone = false;
    private int casts;
    private int hits;
    public override void OnAfterHit(RootWordEffect effect, Caster caster, Caster target, CastResults data)
    {
        // If the caster isn't also the target and the first cast isn't done, this is the first cast and this isn't a self-cast
        if(caster != target && !firstCastDone)
        {
            firstCastDone = true;
            return;
        }
        if (!removeFromHits)
            return;
        if (!firstCastDone && caster == target)
            return;
        // Don't destroy if the attack missed
        if (data.Miss)
            return;
        if(++hits >= hitsToRemove)
            Remove();
    }

    public override void OnAfterCastResolved(Spell s, Caster self)
    {
        if (!removeFromCasts)
            return;
        if(!firstCastDone)
        {
            firstCastDone = true;
            return;
        }
        if(++casts >= castsToRemove)
            Remove();
    }

    public override void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data)
    {
        return;
    }
}
