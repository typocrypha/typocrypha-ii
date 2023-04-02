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
    private PauseHandle parent;
    public bool Pause
    {
        get => pauseCount != 0;
        set
        {
            if (value)
            {
                pauseCount++;
                if(pauseCount == 1)
                {
                    onPause(true);
                }
            }
            else if (pauseCount > 0)
            {
                pauseCount--;
                if(pauseCount == 0)
                {
                    onPause(false);
                }
            }
        }
    }

    public void SetParent(PauseHandle newParent)
    {
        if (newParent == null)
            return;
        FreeFromParent();
        newParent.onPause += onPause;
        parent = newParent;
    }

    private void FreeFromParent()
    {
        if(parent != null)
        {
            parent.onPause -= onPause;
            parent = null;
        }
    }

    /// <summary>
    /// Initialize pause handle.
    /// </summary>
    public PauseHandle(OnPauseDel opd)
    {
        onPause = opd;
        if(PauseManager.instance != null)
        {
            PauseManager.instance.AllPausable.Add(this);
        }

    }

    // Remove self from list of all pause handles on destruction.
    ~PauseHandle()
    {
        if (PauseManager.instance != null)
        {
            PauseManager.instance.AllPausable.Remove(this);
        }
        FreeFromParent();
    }
}
