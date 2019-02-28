using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single action in a battle event.
/// </summary>
[RequireComponent(typeof(BattleEvent))]
public abstract class BattleEventFunction : MonoBehaviour
{
    /// <summary>
    /// Execute function.
    /// </summary>
    public abstract void Run();
}
