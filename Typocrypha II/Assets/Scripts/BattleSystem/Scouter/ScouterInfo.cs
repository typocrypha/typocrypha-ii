using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handler for getting info that comes up when scouter is used.
/// NOT actually a container for the data.
/// </summary>
public abstract class ScouterInfo 
{
    /// <summary>
    /// Primary description text.
    /// </summary>
    public abstract string DescriptionText { get; }

    /// <summary>
    /// Temp image
    /// </summary>
    public Sprite DisplayImage;
}
