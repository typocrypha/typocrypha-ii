using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTagsEffect : RootWordEffect
{
    public CasterTag.TagSet casterTagsToAdd = new CasterTag.TagSet();

    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, bool crit, Spell spell, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target);
        foreach(var t in casterTagsToAdd)
        {
            target.AddTag(t);
        }
        results.Miss = false;
        results.DisplayDamage = false;
        return results;
    }
}
