using UnityEditor;
using System.Linq;
using UnityEngine;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class AudioClipBundleLoader
{
    public const string path = "Assets/ScriptableObjects/Bundles";
    // register an event handler when the class is initialized
    static AudioClipBundleLoader()
    {
        EditorApplication.playModeStateChanged += LoadAudioClipBundles;
        LoadAudioClipBundles(PlayModeStateChange.ExitingPlayMode);
    }

    private static void LoadAudioClipBundles(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode || !AutoRefreshBetterBundles.IsEnabled)
            return;
        LoadAudioClipBundles();
    }

    public static void LoadAudioClipBundles()
    {
        BetterBundleLoaderUtils.LoadBundles<AudioClipBundle, AudioClip>(path);
    }
}
