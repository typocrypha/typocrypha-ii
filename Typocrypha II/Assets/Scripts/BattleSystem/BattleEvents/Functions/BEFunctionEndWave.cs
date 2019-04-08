using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEFunctionEndWave : BattleEventFunction
{
    public override void Run()
    {
        BattleManager.instance.NextWave();
    }
}
