using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAddTagWhileCountered : CasterAbility
{
    [SerializeField] CasterTag tag;
    public override void AddTo(Caster caster)
    {
        if (caster.HasTag(tag))
            return;
        caster.AddTag(tag);
        caster.OnSpellChanged -= OnSpellChange;
        caster.OnSpellChanged += OnSpellChange;
    }

    public override void RemoveFrom(Caster caster)
    {
        caster.RemoveTag(tag);
        caster.OnSpellChanged -= OnSpellChange;
    }

    private void OnSpellChange(Caster caster, Spell s)
    {
        if (!caster.Countered)
        {
            RemoveFrom(caster);
        }
    }
}
