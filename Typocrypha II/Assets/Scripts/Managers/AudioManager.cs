using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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

    public AudioClip this[string clipName] // Allows access to audio clips by name
    {
        get
        {
            return audioBundle.LoadAsset<AudioClip>(clipName);
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
            return;
        }
        DontDestroyOnLoad(gameObject);

        audioBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "audio"));
    }
}
