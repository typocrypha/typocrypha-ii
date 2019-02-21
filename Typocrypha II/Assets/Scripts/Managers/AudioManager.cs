using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Management of audio assets by filename.
/// Also allows simple playback.
/// </summary>
/// <example>
/// <c>AudioManager.instance["clip name"]</c>
/// </example>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null; // Global static instance.
    public AudioSource[] bgm; // Audio sources for playing bgms. Should have 2 audio sources (for crossfading).
    public AudioSource sfx; // Audio source for playing simple sfx.

    AssetBundle audioBundle; // Asset bundle containing all clips.

    public AudioClip this[string clipName] // Allows access to audio clips by name.
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

    /// <summary>
    /// Starts playing audio clip from beginning.
    /// </summary>
    /// <param name="clip">Clip to play as bgm.</param>
    /// <param name="fadeCurve">Volume curve to fade in with.</param>
    public void PlayBGM(AudioClip clip, AnimationCurve fadeCurve = null)
    {
        bgm[0].clip = clip;
        bgm[0].loop = true;
        if (fadeCurve != null)
        {
            StartCoroutine(FadeIn(fadeCurve));
        }
        else
        {
            bgm[0].Play();
        }
    }

    /// <summary>
    /// Fade in currently loaded BGM.
    /// </summary>
    /// <param name="fadeCurve"></param>
    /// <returns></returns>
    IEnumerator FadeIn(AnimationCurve fadeCurve)
    {
        bgm[0].volume = 0f;
        bgm[0].Play();
        float time = 0f;
        while (time < 1f)
        {
            bgm[0].volume = fadeCurve.Evaluate(time);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Pause/Unpause currently playing clip.
    /// </summary>
    /// <param name="pause">True: pause, false: unpause.</param>
    public void PauseBGM(bool pause)
    {
        if (pause) bgm[0].Pause();
        else bgm[0].UnPause();
    }

    /// <summary>
    /// Plays an sfx once.
    /// </summary>
    /// <param name="clip">Clip to play.</param>
    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }
}
