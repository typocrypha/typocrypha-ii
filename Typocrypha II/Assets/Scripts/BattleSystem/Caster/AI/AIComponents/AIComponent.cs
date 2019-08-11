using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class to organize all AI components.
/// Contains a protected reference to the caster.
/// This reference will be initialized if InitializeBase is called
/// </summary>
public abstract class AIComponent : MonoBehaviour
{
    protected Caster caster;
    /// <summary>
    /// Initializes the caster and AI refs, and sets the targeting position to the player
    /// </summary>
    protected void InitializeBase()
    {
        caster = GetComponent<Caster>();
        // Init targeting position
        caster.TargetPos = Battlefield.instance.Player.FieldPos;
    }
}
