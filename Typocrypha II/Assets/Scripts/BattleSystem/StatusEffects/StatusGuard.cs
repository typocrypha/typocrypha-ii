using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusGuard : StatusEffect
{
    bool firstCastDone = false;
    public override void OnAfterHit(RootWordEffect effect, Caster caster, Caster target, CastResults data)
    {
        // Don't destory if this is a guard effect
        var st = (effect as ApplyStatusEffect)?.effectPrefab?.GetComponent<StatusEffect>();
        if (st != null && st.tagToAdd == tagToAdd)
            return;
        // Don't destroy if the attack missed
        if (data.Miss)
            return;
        CleanupAndDestroy();
    }

    public override void OnAfterCastResolved(Spell s, Caster self)
    {
        if(!firstCastDone)
        {
            firstCastDone = true;
            return;
        }
        CleanupAndDestroy();
    }
}
