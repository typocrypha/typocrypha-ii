using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionUnitCountered : BEConditionUnit
{
    protected override bool CheckCaster(Caster caster)
    {
        return caster.Countered;
    }
}
