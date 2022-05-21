using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpiritModeAction : AIComponent
{
    public Spell action;
    public int spiritStagger = 3;

    private void OnEnable()
    {
        caster.OnSpiritMode += SetAction;
    }

    private void OnDisable()
    {
        caster.OnSpiritMode -= SetAction;
    }

    private void SetAction()
    {
        var otherAI = caster.GetComponents<AIComponent>();
        foreach(var ai in otherAI)
        {
            if (ai == this)
                continue;
            ai.enabled = false;
        }
        caster.Spell = action;
        caster.Charge = 0;
        caster.Stats.MaxStagger = spiritStagger;
        caster.Stagger = spiritStagger;
        caster.Stunned = false;
        caster.ChargeTime = action.Cost * caster.Stats.CastingSpeedMod;
        caster.TargetPos = Battlefield.instance.Player.FieldPos;
    }
}
