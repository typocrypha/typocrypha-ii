using System;
using System.IO;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Metadata on a TIPS entry.
/// </summary>
[CreateAssetMenu]
[System.Serializable]
public class TIPSEntryData : ScriptableObject
{
    /*** Old Stuff
     * 
     * public NameSet searchTerms; // Set of all searchable terms.
     * public GameObject entryPrefab; // Prefab object for the entry.
     * 
     ***/

    public const string TIPSFolder = @"Assets/ScriptableObjects/TIPS";

    public string Title;
    public string EntryPath;
    [Multiline(10)] public string EntryText;

    public void OnEnable()
    {
        OnValidate();
    }

    public bool MatchTitlePartial(string query)
    {
        return Title?.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1;
    }

    public bool MatchTitleExact(string query)
    {
        return string.Equals(Title, query, StringComparison.OrdinalIgnoreCase);
    }

    public void OnValidate()
    {
        var projectPath = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(projectPath)) return;

        if (string.IsNullOrEmpty(Title))
        {
            var fileName = Path.GetFileNameWithoutExtension(projectPath);
            Title = fileName;
        }

        var relativePath = projectPath.Replace(TIPSFolder, "");
        EntryPath = Path.GetDirectoryName(relativePath).Replace("\\", "/");
    }
}
