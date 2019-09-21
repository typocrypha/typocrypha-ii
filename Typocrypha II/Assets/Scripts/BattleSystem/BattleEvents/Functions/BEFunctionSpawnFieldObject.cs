using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEFunctionSpawnFieldObject : BattleEventFunction
{
    public Battlefield.Position pos;
    public GameObject fieldObjectPrefab;
    public override void Run()
    {
        var obj = Instantiate(fieldObjectPrefab).GetComponent<FieldObject>();
        Battlefield.instance.Add(obj, pos);
        var actor = obj.GetComponent<ATB3.ATBActor>();
        if(actor != null)
         actor.Pause = false;
    }
}
