using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a single event that can occur during battle.
/// </summary>
public class BattleEvent : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
        enabled = !b; // Disable condition checking.
    }
    #endregion

    public enum Logic
    {
        And, // All conditions must be true.
        Or // Only one condition must be true.
    }

    public Logic logic = Logic.And;
    public bool repeat = false;
    BattleEventCondition[] conditions; // All conditions to check.
    BattleEventFunction[] functions; // All functions to run.
    bool done = false; // Has battle event been executed?

    void Awake()
    {
        ph = new PauseHandle(OnPause);
        conditions = GetComponents<BattleEventCondition>();
        functions = GetComponents<BattleEventFunction>();
    }

    // Check conditions each frame.
    void Update()
    {
        if (done) return;
        if (CheckAll()) RunAll();
    }

    // Check all conditions.
    bool CheckAll()
    {
        if (conditions.Length <= 0)
            return true;
        if (logic == Logic.Or)
        {
            foreach (var cond in conditions)
                if (cond.Check()) return true;
            return false;
        }
        else
        {
            foreach (var cond in conditions)
                if (!cond.Check()) return false;
            return true;
        }
    }

    // Execute all functions.
    void RunAll()
    {
        foreach(var func in functions)
            func.Run();
        if(!repeat)
        {
            done = true;
        }
        else
        {
            foreach (var cond in conditions)
                cond.ResetValues();
        }
    }
}
