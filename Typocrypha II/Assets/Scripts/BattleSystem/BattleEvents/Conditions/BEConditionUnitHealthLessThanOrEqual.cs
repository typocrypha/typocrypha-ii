using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionUnitHealthLessThanOrEqual : BEConditionUnit
{
    [Range(0f, 1f)]
    [SerializeField] float percent = 0;
    protected override bool CheckCaster(Caster caster)
    {
        return ((float)caster.Health / caster.Stats.MaxHP) <= percent;
    }
}
