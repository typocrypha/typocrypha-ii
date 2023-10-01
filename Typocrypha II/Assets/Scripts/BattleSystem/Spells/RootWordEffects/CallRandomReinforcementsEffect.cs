using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallRandomReinforcementsEffect : AbstractCallReinforcementsEffect
{
    public List<GameObject> reinforcementPrefabs;

    protected override GameObject GetReinforcementPrefab(int reinforcementIndex)
    {
        return RandomU.instance.Choice(reinforcementPrefabs);
    }
}
