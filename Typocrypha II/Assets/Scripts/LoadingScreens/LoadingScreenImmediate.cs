using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Immediate loading screen. Attempts to load next scene directly with no transition.
/// </summary>
public class LoadingScreenImmediate : LoadingScreen
{
    private void Start()
    {
        _ready = true;
    }

    public override float Progress
    {
        set
        {
            Debug.Log("loading progress:" + value);
            if (value == 1.0f)
            {
                _done = true;
            }
        }
    }

    // Ready is set when loading screen faded in.
    bool _ready = false;
    public override bool ReadyToLoad
    {
        get
        {
            return _ready;
        }
    }

    // Done is set when scene is loaded.
    bool _done = false;
    public override bool DoneLoading
    {
        get
        {
            return _done;
        }
    }
}
