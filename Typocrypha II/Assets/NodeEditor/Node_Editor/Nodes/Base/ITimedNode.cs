using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for nodes in NodeCanvases that take an amount of time to complete.
/// </summary>
public interface ITimedNode 
{
    /// <summary>
    /// Has this node finished it's effect?
    /// </summary>
    bool IsCompleted { get; }
}
