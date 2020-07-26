using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InceptSpellTagsEffect : RootWordEffect
{
    public List<SpellTag> spellTagsToAdd = new List<SpellTag>();
    public override CastResults Cast(Caster caster, Caster target, RootCastResults prevResults = null)
    {
        target.OnBeforeCastResolved += InceptTag;
        CastResults results = new CastResults(caster, target);
        results.Miss = false;
        results.DisplayDamage = false;
        return results;
    }

    public void InceptTag(RootWordEffect effect, Caster caster, Caster target)
    {
        foreach (var t in spellTagsToAdd)
            effect.tags.Add(t);
        caster.OnBeforeCastResolved -= InceptTag;
    }
}
