using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BetterBundleLoaderUtils
{
    public static void LoadBundles<TBundle,TItem>(string path, Action<TItem> processItem = null) where TBundle : BetterBundle<TItem> where TItem : UnityEngine.Object
    {
        var bundles = AssetUtils.LoadAllAssetsInDirectory<TBundle>(path);
        foreach (var bundle in bundles)
        {
            bundle.Clear();
            foreach (var assetPath in bundle.assetPaths)
            {
                var items = AssetUtils.LoadAllAssetsInDirectoryRecursive<TItem>(assetPath);
                foreach (var item in items)
                {
                    processItem?.Invoke(item);
                    bundle.Add(item);
                }
            }
            EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
