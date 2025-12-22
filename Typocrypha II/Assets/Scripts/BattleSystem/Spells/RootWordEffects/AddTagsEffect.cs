using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTagsEffect : RootWordEffect
{
    public CasterTag.TagSet casterTagsToAdd = new CasterTag.TagSet();

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        var results = InitializeCastResults(caster, target, mod);
        bool success = false;
        foreach (var t in casterTagsToAdd)
        {
            if (target.HasTag(t))
            {
                continue;
            }
            target.AddTag(t);
            success = true;
        }
        results.Miss = !success;
        return results;
    }
}
