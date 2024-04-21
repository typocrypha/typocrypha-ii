using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChangeTagOnHit : CasterAbility
{
    public SpellTag onHitTag;
    public CasterTag removeTag;
    public CasterTag addTag;

    public override void AddTo(Caster caster)
    {
        caster.OnBeforeHitResolved += OnBeforeHitResolved;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.OnBeforeHitResolved -= OnBeforeHitResolved;
    }

    public void OnBeforeHitResolved(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults castResults)
    {
        if (!effect.tags.Contains(onHitTag))
        {
            return;
        }
        target.RemoveTag(removeTag);
        target.AddTag(addTag);
    }
}
