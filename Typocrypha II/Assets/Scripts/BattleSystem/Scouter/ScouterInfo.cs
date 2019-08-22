using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Info that comes up when scouter is used.
/// </summary>
public abstract class ScouterInfo 
{
    /// <summary>
    /// Primary description text.
    /// </summary>
    public abstract string DescriptionText { get; }
}
