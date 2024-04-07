using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnPauseDel(bool b);

[System.Flags]
public enum PauseSources
{
    None = 0,
    PauseMenu = 1,
    GameOver = 2,
    Dialog = 4,
    Battle = 8,
    Parent = 16,
    Self = 32,
    Misc = 64,
    Scouter = 128,
    ATB = 256,
    DialogHistory = 512,
    TIPS = 1024,
}

/// <summary>
/// Wrapper for pause state, allowing for nested pause states. 
/// Used by 'IPausable' interface which requires an 'OnPause' handle to be implemented.
/// </summary>
public class PauseHandle
{
    OnPauseDel onPause; // Function called when paused/unpaused.
    private PauseSources pauseSources;
    private PauseHandle parent;

    public bool Paused
    {
        get => pauseSources != PauseSources.None;
    }
    public void Pause(PauseSources sources)
    {
        bool wasPaused = Paused;
        pauseSources |= sources;
        if(!wasPaused && Paused)
        {
            onPause?.Invoke(true);
        }
    }

    public void Unpause(PauseSources sources)
    {
        bool wasPaused = Paused;
        pauseSources &= ~sources;
        if (wasPaused && !Paused)
        {
            onPause?.Invoke(false);
        }
    }

    public void SetPauseFunction(OnPauseDel function)
    {
        onPause = function;
        onPause?.Invoke(Paused);
    }

    public void SetParent(PauseHandle newParent)
    {
        if (newParent == null)
            return;
        FreeFromParent();
        newParent.onPause += SimpleParentPause;
        parent = newParent;
    }

    public void SetParent(IPausable newParent)
    {
        SetParent(newParent.PH);
    }

    public void PauseIfParentPaused()
    {
        if (parent == null || !parent.Paused)
            return;
        Pause(parent.pauseSources);
    }

    public void SimpleParentPause(bool value)
    {
        if (value)
        {
            Pause(PauseSources.Parent);
        }
        else
        {
            Unpause(PauseSources.Parent);
        }
    }

    public void FreeFromParent()
    {
        if(parent != null)
        {
            parent.onPause -= SimpleParentPause;
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
