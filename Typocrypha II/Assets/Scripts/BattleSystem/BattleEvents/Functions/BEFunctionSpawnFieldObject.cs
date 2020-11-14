using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEFunctionSpawnFieldObject : BattleEventFunction
{
    public Battlefield.Position pos;
    public GameObject fieldObjectPrefab;
    public override void Run()
    {
        StartCoroutine(BattleManager.instance.AddFieldObject(fieldObjectPrefab, pos.Row, pos.Col, true));
    }
}
