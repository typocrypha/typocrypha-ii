using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CasterBundleLoader
{
    public static string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static CasterBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadCasterBundles;
        LoadCasterBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadCasterBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadCasterBundles();
    }

    public static void LoadCasterBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<CasterBundle, GameObject>(path);
    }
}
