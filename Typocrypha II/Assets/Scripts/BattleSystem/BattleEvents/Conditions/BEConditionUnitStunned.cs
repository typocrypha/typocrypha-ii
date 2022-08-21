using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionUnitStunned : BEConditionUnit
{
    protected override bool CheckCaster(Caster caster)
    {
        return caster.Stunned;
    }
}
