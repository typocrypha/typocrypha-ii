using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEFunctionAddEvent : BattleEventFunction
{
    public GameObject battleEventPrefab;
    public override void Run()
    {
        var bEvent = Instantiate(battleEventPrefab).GetComponent<BattleEvent>();
        BattleManager.instance.AddBattleEvent(bEvent);
    }
}
