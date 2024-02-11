using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTagsEffect : RootWordEffect
{
    public CasterTag.TagSet casterTagsToRemove = new CasterTag.TagSet();

    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target)
        {
            DisplayDamage = false
        };
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
