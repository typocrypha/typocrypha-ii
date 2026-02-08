using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISlowdownWithKeyEffects : AIComponent
{
    private int speedMod = 0;

    protected override void AddListeners()
    {
        base.AddListeners();
        caster.OnSpellChanged += OnSpellChanged;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        caster.OnSpellChanged -= OnSpellChanged;
    }

    private void OnSpellChanged(Caster arg1, Spell arg2)
    {
        // Undo previous stat change
        caster.Stats.Spd += speedMod;
        // Calculate speed mod
        var keyEffects = new HashSet<Typocrypha.KeyEffect.EffectType>();
        foreach(var keyEffect in Typocrypha.Keyboard.instance.allEffects)
        {
            if (!keyEffects.Contains(keyEffect.Value.EffectID))
            {
                keyEffects.Add(keyEffect.Value.EffectID);
            }
        }
        speedMod = keyEffects.Count;
        caster.Stats.Spd -= speedMod;
    }
}
