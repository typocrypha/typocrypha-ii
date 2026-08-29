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
    Equipment = 2048,
    Title = 4096,
    Loading = 8192,
}

/// <summary>
/// Wrapper for pause state, allowing for nested pause states. 
/// Used by 'IPausable' interface which requires an 'OnPause' handle to be implemented.
/// </summary>
public class PauseHandle
{
    public bool Persistent { get; }
    public PauseSources PauseSources { get; private set; }
    private OnPauseDel onPause; // Function called when paused/unpaused.
    private PauseHandle parent;

    public bool Paused
    {
        get => PauseSources != PauseSources.None;
    }

    public bool IsPaused() => Paused;

    public void Pause(PauseSources sources)
    {
        bool wasPaused = Paused;
        PauseSources |= sources;
        if(!wasPaused && Paused)
        {
            onPause?.Invoke(true);
        }
    }

    public void Unpause(PauseSources sources)
    {
        bool wasPaused = Paused;
        PauseSources &= ~sources;
        if (wasPaused && !Paused)
        {
            onPause?.Invoke(false);
        }
    }

    public void Unpause()
    {
        if (!Paused)
            return;
        PauseSources = PauseSources.None;
        onPause?.Invoke(false);
    }

    public PauseSources UnpauseOverride()
    {
        var temp = PauseSources;
        Unpause(PauseSources);
        return temp;
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
        Pause(parent.PauseSources);
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

    public void Cleanup()
    {
        if (PauseManager.instance != null)
        {
            PauseManager.instance.AllPausable.Remove(this);
        }
        FreeFromParent();
    }

    public PauseHandle(bool persistent = false)
    {
        Persistent = persistent;
        if (PauseManager.instance != null)
        {
            PauseManager.instance.AllPausable.Add(this);
        }
        else
        {
            Debug.LogError("PH not hooked up");
        }
    }

    /// <summary>
    /// Initialize pause handle.
    /// </summary>
    public PauseHandle(OnPauseDel opd, bool persistent = false) : this(persistent)
    {
        onPause = opd;
    }

    // Remove self from list of all pause handles on destruction.
    ~PauseHandle()
    {
        Cleanup();
    }
}
