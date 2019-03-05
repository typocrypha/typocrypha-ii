using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Metadata on a TIPS entry.
/// </summary>
[CreateAssetMenu]
[System.Serializable]
public class TIPSEntryData : ScriptableObject
{
    public NameSet searchTerms; // Set of all searchable terms.
    public GameObject entryPrefab; // Prefab object for the entry.
}
