using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for pausable objects.
/// </summary>
public interface IPausable 
{
    /// <summary>
    /// Pause property: set to 'true' to pause, or 'false' to unpause.
    /// </summary>
    bool Pause
    {
        get;
        set;
    }
}
