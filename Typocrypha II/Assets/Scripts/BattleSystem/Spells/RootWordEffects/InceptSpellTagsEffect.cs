using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InceptSpellTagsEffect : RootWordEffect
{
    public List<SpellTag> spellTagsToAdd = new List<SpellTag>();

    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        target.OnBeforeSpellEffectCast += InceptTag;
        CastResults results = new CastResults(caster, target)
        {
            Miss = false,
            DisplayDamage = false
        };
        return results;
    }

    public void InceptTag(RootWordEffect effect, Caster caster, Caster target)
    {
        foreach (var t in spellTagsToAdd)
            effect.tags.Add(t);
        caster.OnBeforeSpellEffectCast -= InceptTag;
    }
}
