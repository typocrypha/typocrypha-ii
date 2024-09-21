using System;
using System.Collections;
using System.Collections.Generic;
using Typocrypha;
using UnityEngine;

public class BEConditionKeyEffectApplied : BattleEventCondition
{
    [SerializeField] private Typocrypha.KeyEffect.EffectType effectId;
    private bool applied = false;
    protected override bool CheckInternal()
    {
        return applied;
    }

    protected override void AddEventHandlers()
    {
        base.AddEventHandlers();
        Keyboard.instance.OnKeyEffectApplied -= CheckKeyEffect;
        Keyboard.instance.OnKeyEffectApplied += CheckKeyEffect;
    }

    protected override void RemoveEventHandlers()
    {
        base.RemoveEventHandlers();
        Keyboard.instance.OnKeyEffectApplied -= CheckKeyEffect;
    }

    private void CheckKeyEffect(KeyEffect obj)
    {
        if (applied)
            return;
        if(effectId == KeyEffect.EffectType.None || obj.EffectID == effectId)
        {
            applied = true;
        }
    }

    public override void ResetValues()
    {
        base.ResetValues();
        applied = false;
    }
}
