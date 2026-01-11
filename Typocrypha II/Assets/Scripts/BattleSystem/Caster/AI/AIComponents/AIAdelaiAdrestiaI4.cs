using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAdelaiAdrestiaI4 : AIFollowUpOnPlayerCounter
{
    [SerializeField] private AIComponent autoAI;
    bool moved = false;
    protected override void AddListeners()
    {
        base.AddListeners();
        BattleManager.instance.OnBattleEventTriggered += OnBattleEventTriggered;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        BattleManager.instance.OnBattleEventTriggered -= OnBattleEventTriggered;
        SpellManager.instance.OnBeforeSpellTravelFx -= GuardPlayer;
        SpellManager.instance.OnAfterCastResolved -= ReturnToSpace;
    }

    private void OnBattleEventTriggered(string id)
    {
        if (id == "AdelaiFullAuto")
        {
            RemoveListeners();
            autoAI.enabled = true;
            Battlefield.instance.Player.Protector = caster;
            SpellManager.instance.OnBeforeSpellTravelFx += GuardPlayer;
            SpellManager.instance.OnAfterCastResolved += ReturnToSpace;
        }
    }

    private void ReturnToSpace()
    {
        if (moved)
        {
            moved = false;
            AllyBattleBoxManager.instance.ReturnBattleBox(0.75f);
        }
    }

    private void GuardPlayer(Caster caster, Battlefield.Position target)
    {
        if(Battlefield.instance.GetCaster(target) == Battlefield.instance.Player && !caster.IsSpiritMode)
        {
            AllyBattleBoxManager.instance.MoveBattleBox(Battlefield.instance.GetSpace(target) + new Vector2(0, 100), 0.3f);
            moved = true;
        }
    }
}
