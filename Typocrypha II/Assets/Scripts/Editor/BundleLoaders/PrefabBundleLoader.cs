using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class PrefabBundleLoader
{
    public static string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static PrefabBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadPrefabBundles;
        LoadPrefabBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadPrefabBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadPrefabBundles();
    }

    public static void LoadPrefabBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<PrefabBundle, GameObject>(path);
    }
}
