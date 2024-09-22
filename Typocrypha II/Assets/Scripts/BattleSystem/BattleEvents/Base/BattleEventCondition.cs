using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for a condition to check for a battle event.
/// </summary>
[RequireComponent(typeof(BattleEvent))]
public abstract class BattleEventCondition : MonoBehaviour
{
    [SerializeField] private bool invert;
    protected BattleEvent battleEvent; // Battle event management code.

    protected void Awake()
    {
        battleEvent = GetComponent<BattleEvent>();
    }

    protected virtual void Start()
    {
        AddEventHandlers();
        Initialize();
    }

    protected void OnDestroy()
    {
        RemoveEventHandlers();
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void AddEventHandlers()
    {

    }

    protected virtual void RemoveEventHandlers()
    {

    }

    /// <summary>
    /// Return whether condition has been satisfied.
    /// </summary>
    /// <returns>Whether condition has been satisfied or not.</returns>
    public bool Check()
    {
        return invert ^ CheckInternal();
    }

    protected abstract bool CheckInternal();

    public virtual void ResetValues() { }
}
