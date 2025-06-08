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
    [Header("Validated Properties")]
    public string Title;
    public string Parent;
    public string RelativePath;
    public int Depth;
    public bool IsFolder;
    public bool CreateFolder;


    [Header("Manual Properties")]
    [Space(10)]
    [Multiline(10)] public string Content;

    public const string BASE_ASSET_PATH = "ScriptableObjects/TIPS";
    public string[] Categorization => RelativePath.Split('\\');

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
        // Important paths
        var pathToEntry = AssetDatabase.GetAssetPath(this);
        var pathToParent = new FileInfo(pathToEntry).DirectoryName;
        var pathToTIPS = Path.Combine(Application.dataPath, BASE_ASSET_PATH) + "/";

        // Handle folder fields
        IsFolder = new DirectoryInfo(Path.Combine(pathToParent, Title)).Exists;
        if (!IsFolder && CreateFolder)
        {
            Directory.CreateDirectory(Path.Combine(pathToParent, Title));
            AssetDatabase.Refresh();
        }
        CreateFolder = false;

        // Update other validated fields
        Title = Path.GetFileNameWithoutExtension(pathToEntry);
        Parent = Path.GetFileNameWithoutExtension(pathToParent);
        RelativePath = pathToParent.Substring(Mathf.Min(pathToTIPS.Length, pathToParent.Length));
        Depth = Categorization.Length - 1;
    }
}
