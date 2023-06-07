using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpiritModeAction : AIComponent
{
    public Spell action;
    public int spiritStagger = 3;
    [SerializeField] private bool resetToRunAfterCast = true;

    private void OnEnable()
    {
        caster.OnSpiritMode += SetAction;
        caster.OnAfterCastResolved += ResetCast;
    }

    private void OnDisable()
    {
        caster.OnSpiritMode -= SetAction;
        caster.OnAfterCastResolved -= ResetCast;
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
        ChangeSpell(action);
        caster.Stats.MaxStagger = spiritStagger;
        caster.Stagger = spiritStagger;
        caster.Stunned = false;
        caster.TargetPos = Battlefield.instance.Player.FieldPos;
    }

    private void ResetCast(Spell spell, Caster self)
    {
        if (caster.BStatus == Caster.BattleStatus.SpiritMode && resetToRunAfterCast)
        {
            ChangeSpell(action);
        }
    }
}
