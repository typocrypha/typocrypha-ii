using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BEFunctionSpawnReinforcements : BEFunctionSpawnReinforcementsAbstract
{
    [SerializeField] private bool consume = true;
    protected override bool Consume => consume;
}
