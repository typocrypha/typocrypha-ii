using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for Scouter Data container.
/// </summary>
[CreateAssetMenu(fileName = "ScouterData", menuName = "ScouterData")]
public class ScouterData : ScriptableObject
{
    public string description;
    public Sprite pic;
}
