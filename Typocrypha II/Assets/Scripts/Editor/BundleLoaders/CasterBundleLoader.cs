using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CasterBundleLoader : MonoBehaviour
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
        var prefabBundles = AssetUtils.LoadAllAssetsInDirectory<CasterBundle>(path);
        foreach (var bundle in prefabBundles)
        {
            bundle.prefabs.Clear();
            var prefabs = AssetUtils.LoadAllAssetsInDirectoryRecursive<GameObject>(bundle.assetPath);
            foreach (var prefab in prefabs)
            {
                var caster = prefab.GetComponent<Caster>();
                if (caster == null || string.IsNullOrEmpty(caster.DisplayName))
                    continue;
                if (!bundle.prefabs.ContainsKey(caster.DisplayName))
                    bundle.prefabs.Add(caster.DisplayName, prefab);
            }
            if (bundle != null)
                EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
