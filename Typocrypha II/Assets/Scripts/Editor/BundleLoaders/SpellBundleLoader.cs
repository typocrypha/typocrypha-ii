using UnityEditor;
using System.Linq;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class LoadSpellBundlesOnPlayMode
{
    public static string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static LoadSpellBundlesOnPlayMode()
    {
        EditorApplication.playModeStateChanged += LoadSpellBundles;
        LoadSpellBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadSpellBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
            return;
        var spellBundles = AssetUtils.LoadAllAssetsInDirectory<SpellWordBundle>(path);
        foreach (var bundle in spellBundles)
        {
            bundle.words.Clear();
            var words = AssetUtils.LoadAllAssetsInDirectoryRecursive<SpellWord>(bundle.assetPath);
            foreach (var word in words)
            {
                if(!bundle.words.ContainsKey(word.internalName))
                    bundle.words.Add(word.internalName, word);
            }
            if(bundle != null)
                EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
