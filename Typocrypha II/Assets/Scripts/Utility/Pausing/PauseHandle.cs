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
                    onPause?.Invoke(true);
                }
            }
            else if (pauseCount > 0)
            {
                pauseCount--;
                if(pauseCount == 0)
                {
                    onPause?.Invoke(false);
                }
            }
        }
    }

    public void SetPauseFunction(OnPauseDel function)
    {
        onPause = function;
        onPause?.Invoke(Pause);
    }

    public void SetParent(PauseHandle newParent)
    {
        if (newParent == null)
            return;
        FreeFromParent();
        newParent.onPause += SetPause;
        parent = newParent;
    }

    private void SetPause(bool pauseState)
    {
        Pause = pauseState;
    }

    public void SetParent(IPausable newParent)
    {
        SetParent(newParent.PH);
    }

    public void PauseIfParentPaused()
    {
        if (parent == null || !parent.Pause)
            return;
        this.Pause = true;
    }

    private void FreeFromParent()
    {
        if(parent != null)
        {
            parent.onPause -= SetPause;
            parent = null;
        }
    }

    public PauseHandle()
    {
        if (PauseManager.instance != null)
        {
            PauseManager.instance.AllPausable.Add(this);
        }
    }

    /// <summary>
    /// Initialize pause handle.
    /// </summary>
    public PauseHandle(OnPauseDel opd) : this()
    {
        onPause = opd;
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
