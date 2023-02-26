using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BEFunctionSpawnReinforcementsCycle : BEFunctionSpawnReinforcementsAbstract
{
    protected override bool Consume => false;
    private int index = -1;

    protected override int GetReinforcementIndex(List<GameObject> reinforcements)
    {
        if(++index >= reinforcements.Count)
        {
            index = 0;
        }
        return index;
    }
}
