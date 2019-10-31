using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChangeTagOnHit : CasterAbility
{
    public SpellTag onHitTag;
    public CasterTag removeTag;
    public CasterTag addTag;

    public override void OnCast(Spell spell, RootWordEffect effect, Caster caster, Caster target)
    {

    }

    public override void OnHit(Spell spell, RootWordEffect effect, Caster caster, Caster target, CastResults castResults)
    {
        //if (contains slashing)
        {
            target.RemoveTag(removeTag);
            target.AddTag(addTag);
        }
        return;
    }
}
