using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single action in a battle event.
/// </summary>
[RequireComponent(typeof(BattleEvent))]
public abstract class BattleEventFunction : MonoBehaviour
{
    protected BattleEvent battleEvent; // Battle event management code.

    protected void Awake()
    {
        battleEvent = GetComponent<BattleEvent>();
    }

    /// <summary>
    /// Execute function.
    /// </summary>
    public abstract void Run();
}
