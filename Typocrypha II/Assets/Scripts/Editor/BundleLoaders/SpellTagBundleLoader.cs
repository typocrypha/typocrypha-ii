using UnityEditor;
using System.Linq;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class LoadSpellTagBundlesOnPlayMode
{
    public static string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static LoadSpellTagBundlesOnPlayMode()
    {
        EditorApplication.playModeStateChanged += LoadSpellTagBundles;
        LoadSpellTagBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadSpellTagBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
            return;
        var spellBundles = AssetUtils.LoadAllAssetsInDirectory<SpellTagBundle>(path);
        foreach (var bundle in spellBundles)
        {
            bundle.tags.Clear();
            var words = AssetUtils.LoadAllAssetsInDirectory<SpellTag>(bundle.assetPath);
            foreach (var word in words)
            {
                if(!bundle.tags.ContainsKey(word.internalName))
                    bundle.tags.Add(word.internalName, word);
            }
            if(bundle != null)
                EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
