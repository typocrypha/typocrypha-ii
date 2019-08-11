using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTagsEffect : RootWordEffect
{
    public CasterTag.TagSet casterTagsToAdd = new CasterTag.TagSet();
    public override CastResults Cast(Caster caster, Caster target)
    {
        CastResults results = new CastResults(caster, target);
        foreach(var t in casterTagsToAdd)
        {
            target.Tags.Add(t);
        }
        results.Miss = false;
        results.DisplayDamage = false;
        return results;
    }
}
