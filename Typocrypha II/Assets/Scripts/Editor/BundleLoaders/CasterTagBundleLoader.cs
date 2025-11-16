using UnityEditor;
using System.Linq;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class CasterTagBundleLoader
{
    public static string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static CasterTagBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadCasterTagBundles;
        LoadCasterTagBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadCasterTagBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadCasterTagBundles();
    }

    public static void LoadCasterTagBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<CasterTagBundle, CasterTag>(path);
    }
}
