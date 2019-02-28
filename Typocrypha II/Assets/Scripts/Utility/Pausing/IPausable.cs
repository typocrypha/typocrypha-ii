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
