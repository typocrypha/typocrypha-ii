using UnityEditor;

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
        BetterBundleLoaderUtils.LoadBundles<TIPSBundle, TIPSEntryData>(path, ValidateTIPSEntry);
    }

    private static void ValidateTIPSEntry(TIPSEntryData entry)
    {
        entry.OnValidate();
    }
}
