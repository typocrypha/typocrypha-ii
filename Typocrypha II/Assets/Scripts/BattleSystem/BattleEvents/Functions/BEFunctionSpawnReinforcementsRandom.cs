using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BEFunctionSpawnReinforcementsRandom : BEFunctionSpawnReinforcementsAbstract
{
    protected override bool Consume => false;

    protected override int GetReinforcementIndex(List<GameObject> reinforcements)
    {
        return RandomUtils.RandomU.instance.RandomInt(0, reinforcements.Count);
    }
}
