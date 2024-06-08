using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIResetTargetToPlayer : AIComponent
{
    private void OnEnable()
    {
        caster.OnAfterCastResolved += (spell, self, hitTarget) => ResetTarget();
    }

    private void OnDisable()
    {
        caster.OnAfterCastResolved -= (spell, self, hitTarget) => ResetTarget();
    }

    public void ResetTarget()
    {
        caster.TargetPos = Battlefield.instance.Player.FieldPos;
    }
}
