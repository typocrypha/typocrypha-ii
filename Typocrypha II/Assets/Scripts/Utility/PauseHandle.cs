using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper for pause state, allowing for nested pauss states. 
/// Used primarily by 'IPausable' interface.
/// </summary>
public class PauseHandle
{
    int pauseCount = 0; // Number of nested pause states.

    public bool Pause
    {
        get => pauseCount != 0;
        set
        {
            if (value)
            {
                pauseCount++;
            }
            else
            {
                if (pauseCount > 0)
                {
                    pauseCount--;
                }
            }
        }
    }
}
