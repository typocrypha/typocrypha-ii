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
        if (state != PlayModeStateChange.ExitingEditMode)
            return;
        LoadPrefabBundles();
    }

    public static void LoadPrefabBundles()
    {
        var spellBundles = AssetUtils.LoadAllAssetsInDirectory<PrefabBundle>(path);
        foreach (var bundle in spellBundles)
        {
            bundle.prefabs.Clear();
            var prefabs = AssetUtils.LoadAllAssetsInDirectoryRecursive<GameObject>(bundle.assetPath);
            foreach (var prefab in prefabs)
            {
                if (!bundle.prefabs.ContainsKey(prefab.name))
                    bundle.prefabs.Add(prefab.name, prefab);
            }
            if (bundle != null)
                EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
