using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for custom loading screen effects.
/// </summary>
public abstract class LoadingScreen : MonoBehaviour
{
    /// <summary>
    /// Handle for setting progress of load. 
    /// Put effects (progress bars etc) based on load progress here.
    /// Progress will stop at 0.9 when done loading, and then go to 1.0 when finialized.
    /// </summary>
    public abstract float Progress
    {
        set;
    }

    /// <summary>
    /// Returns true if ready to start loading scene.
    /// </summary>
    public abstract bool ReadyToLoad
    {
        get;
    }

    /// <summary>
    /// Returns true if ready to load next scene.
    /// </summary>
    public abstract bool DoneLoading
    {
        get;
    }
}
