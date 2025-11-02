using System;
using System.IO;
using System.Collections.Generic;
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
    [Header("Validated Properties")]
    public string ID;
    public string Parent;
    public string RelativePath;
    public int Depth;
    public bool IsFolder;
    public bool CreateFolder;

    [Header("Manual Properties")]
    [TextArea(10, 3)] public string Content;
    [TextArea(5, 3)] public string Footer;
    public List<string> Aliases;

    public const string BASE_ASSET_PATH = "ScriptableObjects/TIPS";
    public string[] Categorization => RelativePath.Trim('\\'). Split('\\');
    public static string PathToTIPS => Path.Combine(Application.dataPath, BASE_ASSET_PATH);

    public virtual string Title => ID;

    public bool MatchTitlePartial(string query)
    {
        return ID?.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1;
    }

    public bool MatchTitleExact(string query)
    {
        return string.Equals(ID, query, StringComparison.OrdinalIgnoreCase);
    }

    public bool MatchAliasExact(string query)
    {
        foreach (var alias in Aliases)
        {
            if (string.Equals(alias, query, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    public virtual void OnValidate()
    {
        // Important paths
        var pathToEntry = AssetDatabase.GetAssetPath(GetInstanceID());
        if (string.IsNullOrEmpty(pathToEntry)) return;
        ID = Path.GetFileNameWithoutExtension(pathToEntry);

        var pathToParent = new FileInfo(pathToEntry).DirectoryName;
        Parent = Path.GetFileNameWithoutExtension(pathToParent);

        // Handle folder fields
        IsFolder = new DirectoryInfo(Path.Combine(pathToParent, ID)).Exists;
        if (!IsFolder && CreateFolder)
        {
            Directory.CreateDirectory(Path.Combine(pathToParent, ID));
            AssetDatabase.Refresh();
        }
        CreateFolder = false;

        // Update other validated fields
        RelativePath = pathToParent.Substring(Mathf.Min(PathToTIPS.Length, pathToParent.Length));
        Depth = Categorization.Length - 1;
    }
}
