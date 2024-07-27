using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusRemoveAfterSpellChange : StatusEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        affected.OnSpellChanged -= OnSpellChanged;
        affected.OnSpellChanged += OnSpellChanged;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        affected.OnSpellChanged -= OnSpellChanged;
        affected.OnSpellChanged += OnSpellChanged;
    }

    private void OnSpellChanged(Caster self, Spell newSpell)
    {
        Remove();
    }

    public override string FailMessage(Caster caster, Caster target)
    {
        return $"{target.DisplayName} is already {casterTag.DisplayName}!";
    }
}
