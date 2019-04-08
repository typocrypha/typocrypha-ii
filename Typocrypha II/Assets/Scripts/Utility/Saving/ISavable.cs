using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for an object to have a save state and be able to rebuild from a save.
/// </summary>
public interface ISavable 
{
    /// <summary>
    /// Saves a serializable representation of the state of this object to loaded data.
    /// </summary>
    void Save();

    /// <summary>
    /// Loads serializable state from loaded data.
    /// </summary>
    void Load();
}
