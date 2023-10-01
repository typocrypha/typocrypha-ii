using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallReinforcementsEffect : AbstractCallReinforcementsEffect
{
    public GameObject reinforcementPrefab;

    protected override GameObject GetReinforcementPrefab(int reinforcementIndex)
    {
        return reinforcementPrefab;
    }
}
