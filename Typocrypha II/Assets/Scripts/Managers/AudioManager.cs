using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

/// <summary>
/// Management of audio assets by filename.
/// Not for playback use.
/// </summary>
/// <example>
/// <c>AudioManager.instance["clip name"]</c>
/// </example>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null; // Global static instance

    AssetBundle audioBundle; // Asset bundle containing all clips
    Dictionary<string, AudioClip> clips; // Maps filename to loaded Audio Clips

    public AudioClip this[string clipName] // Allows access to audio clips by name
    {
        get
        {
            Load(clipName);
            return clips[clipName];
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (audioBundle != null)
        {
            return;
        }
        audioBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "audio"));
        clips = new Dictionary<string, AudioClip>();
    }

    /// <summary>
    /// Load clip by name if not already loaded.
    /// </summary>
    public void Load(string clipName)
    {
        if (!clips.ContainsKey(clipName))
        {
            if (audioBundle.Contains(clipName))
            {
                clips.Add(clipName, audioBundle.LoadAsset<AudioClip>(clipName));
            }
            else
            {
                throw new System.Exception("Trying to load audio asset that doesnt exist:" + clipName);
            }
        }
    }

    /// <summary>
    /// Load all AudioClips.
    /// </summary>
    public void LoadAll()
    {
        AudioClip[] allClips = audioBundle.LoadAllAssets<AudioClip>();
        foreach (AudioClip clip in allClips)
        {
            if (!clips.ContainsKey(clip.name))
            {
                clips.Add(clip.name, clip);
            }
        }
    }
}
