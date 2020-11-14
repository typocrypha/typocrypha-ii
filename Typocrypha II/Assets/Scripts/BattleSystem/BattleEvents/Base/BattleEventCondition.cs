using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for a condition to check for a battle event.
/// </summary>
[RequireComponent(typeof(BattleEvent))]
public abstract class BattleEventCondition : MonoBehaviour
{
    protected BattleEvent battleEvent; // Battle event management code.

    protected void Awake()
    {
        battleEvent = GetComponent<BattleEvent>();
    }

    /// <summary>
    /// Return whether condition has been satisfied.
    /// </summary>
    /// <returns>Whether condition has been satisfied or not.</returns>
    public abstract bool Check();

    public virtual void ResetValues() { }
}
