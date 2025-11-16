using UnityEditor;
using System.Linq;
using System.IO;

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
        LoadSpellBundles();
    }

    public static void LoadSpellBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<BadgeBundle, BadgeWord>(path);
    }
}
