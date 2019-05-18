using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for pausable objects.
/// Proper implementation: 
/// 
/// 
/// PauseHandle ph; // Internal pause handle.
/// public PauseHandle PH { get => ph; } // Accessor.
/// 
/// public void OnPause(bool b) 
/// {
///     // Called when object is paused/unpaused.
/// }
/// 
/// Constructor/Awake() 
/// {
///     ph = new PauseHandle(OnPause);
///     // ... etc ...
/// }
/// 
/// 
/// </summary>
public interface IPausable 
{
    /// <summary>
    /// Pause handle for handling nested pausing and global pausing.
    /// Usage: [object].PH.Pause = true/false
    /// </summary>
    PauseHandle PH { get; }

    /// <summary>
    /// This is called by 'PauseHandle' when object is paused(true)/unpaused(false).
    /// </summary>
    void OnPause(bool b);
}

/// <summary>
/// Custom yield instruction: waits for certain amount of seconds, but also pauses.
/// </summary>
public class WaitForSecondsPause : CustomYieldInstruction
{
    PauseHandle ph; // Pausing interface
    float seconds; // Number of seconds to wait

    public override bool keepWaiting
    {
        get
        {
            if (!ph.Pause) seconds -= Time.deltaTime;
            return seconds >= 0f;
        }
    }

    public WaitForSecondsPause(float seconds, PauseHandle ph)
    {
        this.ph = ph;
        this.seconds = seconds;
    }
}