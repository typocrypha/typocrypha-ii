using UnityEditor;
using System.Linq;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class SpellBundleLoader
{
    public static string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static SpellBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadSpellBundles;
        LoadSpellBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadSpellBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadSpellBundles();
    }

    public static void LoadSpellBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<SpellWordBundle, SpellWord>(path);
    }
}
