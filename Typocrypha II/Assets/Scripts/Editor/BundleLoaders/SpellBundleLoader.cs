using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class LoadSpellBundlesOnPlayMode
{
    // register an event handler when the class is initialized
    static LoadSpellBundlesOnPlayMode()
    {
        EditorApplication.playModeStateChanged += LoadSpellBundles;
        LoadSpellBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadSpellBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingPlayMode)
            return;
        var spellBundles = AssetUtils.LoadAllAssetsInDirectory<SpellWordBundle>("Assets/ScriptableObjects/Bundles");
        foreach (var bundle in spellBundles)
        {
            bundle.words.Clear();
            var words = AssetUtils.LoadAllAssetsInDirectory<SpellWord>(bundle.assetPath);
            foreach (var word in words)
            {
                if(!bundle.words.ContainsKey(word.displayName))
                    bundle.words.Add(word.displayName, word);
            }
            if(bundle != null)
                EditorUtility.SetDirty(bundle);
        }
        AssetDatabase.SaveAssets();
    }
}
