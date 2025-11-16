using UnityEditor;
using System.Linq;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class SpellTagBundleLoader
{
    public static string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static SpellTagBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadSpellTagBundles;
        LoadSpellTagBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadSpellTagBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadSpellTagBundles();
    }

    public static void LoadSpellTagBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<SpellTagBundle, SpellTag>(path);
    }
}
