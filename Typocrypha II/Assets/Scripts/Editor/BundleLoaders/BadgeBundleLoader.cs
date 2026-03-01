using UnityEditor;
using System.Linq;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class BadgeBundleLoader
{
    public const string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static BadgeBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadBadgeBundles;
        LoadBadgeBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadBadgeBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadBadgeBundles();
    }

    public static void LoadBadgeBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<BadgeBundle, BadgeWord>(path);
    }

    public static BadgeBundle GetBadgeBundle(string name = null)
    {
        var bundles = AssetUtils.LoadAllAssetsInDirectory<BadgeBundle>(path);
        if (string.IsNullOrEmpty(name))
            return bundles.FirstOrDefault();
        foreach(var bundle in bundles)
        {
            if (bundle.name.ToLower() == name)
                return bundle;
        }
        return null;
    }
}
