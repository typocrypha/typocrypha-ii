using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTagsEffect : RootWordEffect
{
    public CasterTag.TagSet casterTagsToRemove = new CasterTag.TagSet();

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        var results = InitializeCastResults(caster, target, mod);
        bool success = false;
        foreach (var t in casterTagsToRemove)
        {
            if (!target.HasTag(t))
            {
                continue;
            }
            target.RemoveTag(t);
            success = true;
        }
        results.Miss = !success;
        return results;
    }
}
