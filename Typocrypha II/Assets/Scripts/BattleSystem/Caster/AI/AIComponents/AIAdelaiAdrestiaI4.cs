using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAdelaiAdrestiaI4 : AIFollowUpOnPlayerCounter
{
    [SerializeField] private AIComponent autoAI;
    protected override void AddListeners()
    {
        base.AddListeners();
        BattleManager.instance.OnBattleEventTriggered += OnBattleEventTriggered;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        BattleManager.instance.OnBattleEventTriggered -= OnBattleEventTriggered;
    }

    private void OnBattleEventTriggered(string id)
    {
        if (id == "AdelaiFullAuto")
        {
            RemoveListeners();
            autoAI.enabled = true;
        }
    }
}
