using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class AssetUtils
{
    public static List<T> LoadAllAssetsInDirectory<T>(string path) where T : UnityEngine.Object
    {
        var ret = new List<T>();
        var files = Directory.EnumerateFiles(path);
        foreach (var file in files)
        {
            if (file.Contains(".meta"))
                continue;
            string assetPath = file;
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if(asset != null)
                ret.Add(asset);
        }
        return ret;
    }

    public static List<T> LoadAllAssetsInDirectoryRecursive<T>(string path) where T : UnityEngine.Object
    {
        var ret = new List<T>();
        var files = Directory.EnumerateFiles(path);
        foreach (var file in files)
        {
            if (file.Contains(".meta"))
                continue;
            string assetPath = file;
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
                ret.Add(asset);
        }
        var folders = Directory.EnumerateDirectories(path);
        foreach(var folder in folders)
        {
            ret.AddRange(LoadAllAssetsInDirectoryRecursive<T>(folder));
        }
        return ret;
    }
}
