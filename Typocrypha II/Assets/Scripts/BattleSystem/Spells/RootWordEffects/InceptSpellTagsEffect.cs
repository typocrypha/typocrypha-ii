using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InceptSpellTagsEffect : RootWordEffect
{
    public List<SpellTag> spellTagsToAdd = new List<SpellTag>();

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        target.OnBeforeSpellEffectCast += InceptTag;
        return InitializeCastResults(caster, target, mod);
    }

    public void InceptTag(RootWordEffect effect, Caster caster, Caster target, Damage.DamageModifier mod)
    {
        foreach (var t in spellTagsToAdd)
            effect.tags.Add(t);
        caster.OnBeforeSpellEffectCast -= InceptTag;
    }
}
