using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Immediate loading screen. Attempts to load next scene directly with no transition.
/// </summary>
public class LoadingScreenImmediate : LoadingScreen
{
    public override float Progress { set { } }

    public override Coroutine FinishLoading()
    {
        return null;
    }

    public override Coroutine StartLoading()
    {
        return null;
    }
}
