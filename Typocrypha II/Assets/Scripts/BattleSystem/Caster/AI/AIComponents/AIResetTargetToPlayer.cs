using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIResetTargetToPlayer : AIComponent
{
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize the standard refs (caster and AI)
        InitializeBase();
    }

    private void OnEnable()
    {
        caster.OnAfterCastResolved += (spell, self) => ResetTarget();
    }

    private void OnDisable()
    {
        caster.OnAfterCastResolved -= (spell, self) => ResetTarget();
    }

    public void ResetTarget()
    {
        caster.TargetPos = Battlefield.instance.Player.FieldPos;
    }
}
