using UnityEditor;
using System.Linq;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class TIPSBundleLoader
{
    public const string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static TIPSBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadTIPSBundlesOnStateChange;
        LoadTIPSBundlesOnStateChange(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadTIPSBundlesOnStateChange(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadTIPSBundles();
    }

    public static void LoadTIPSBundles()
    {
        var bundles = AssetUtils.LoadAllAssetsInDirectory<TIPSBundle>(path);
        foreach (var bundle in bundles)
        {
            bundle.entries.Clear();
            var incomingEntries = AssetUtils.LoadAllAssetsInDirectoryRecursive<TIPSEntryData>(bundle.assetPath);
            foreach (var entry in incomingEntries)
            {
                if (!bundle.entries.ContainsKey(entry.Title))
                    bundle.entries.Add(entry.Title, entry);
            }
            EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
