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
        if (state != PlayModeStateChange.ExitingEditMode)
            return;
        LoadSpellBundles();
    }

    public static void LoadSpellBundles()
    {
        var spellBundles = AssetUtils.LoadAllAssetsInDirectory<BadgeBundle>(path);
        foreach (var bundle in spellBundles)
        {
            bundle.badges.Clear();
            var words = AssetUtils.LoadAllAssetsInDirectoryRecursive<EquipmentWord>(bundle.assetPath);
            foreach (var word in words)
            {
                if (!bundle.badges.ContainsKey(word.Key))
                    bundle.badges.Add(word.Key, word);
            }
            if (bundle != null)
                EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
