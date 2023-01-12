using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnPauseDel(bool b);

/// <summary>
/// Wrapper for pause state, allowing for nested pause states. 
/// Used by 'IPausable' interface which requires an 'OnPause' handle to be implemented.
/// </summary>
public class PauseHandle
{
    OnPauseDel onPause; // Function called when paused/unpaused.
    int pauseCount = 0; // Number of nested pause states.

    public bool IsPaused() => Pause;
    public bool Pause
    {
        get => pauseCount != 0;
        set
        {
            if (pauseCount == 0 && value) onPause(true);
            if (pauseCount == 1 && !value) onPause(false);
            if (value) pauseCount++;
            else if (pauseCount > 0) pauseCount--;
        }
    }

    /// <summary>
    /// Initialize pause handle.
    /// </summary>
    public PauseHandle(OnPauseDel opd)
    {
        onPause = opd;
        PauseManager.instance?.allPausable.Add(this);
    }

    // Remove self from list of all pause handles on destruction.
    ~PauseHandle()
    {
        PauseManager.instance?.allPausable.Remove(this);
    }
}
