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
public class AudioManager : MonoBehaviour, ISavable
{
    #region ISavable
    public void Save()
    {
        if (bgm[bgmInd].isPlaying)
            SaveManager.instance.loaded.bgm = bgm[bgmInd].clip.name;
    }

    public void Load()
    {
        PlayBGM(this[SaveManager.instance.loaded.bgm]);
    }
    #endregion

    public static AudioManager instance = null; // Global static instance.
    public AudioSource[] bgm; // Audio sources for playing bgms. Should have 2 audio sources (for crossfading).
    public AudioSource sfx; // Audio source for playing simple sfx.

    AssetBundle audioBundle; // Asset bundle containing all clips.
    int bgmInd; // Index of in use bgm audio source.

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
        bgmInd = 0;
    }

    /// <summary>
    /// Starts playing audio clip from beginning.
    /// </summary>
    /// <param name="clip">Clip to play as bgm.</param>
    /// <param name="fadeCurve">Volume curve to fade in with.</param>
    public void PlayBGM(AudioClip clip, AnimationCurve fadeCurve = null)
    {
        bgm[bgmInd].clip = clip;
        bgm[bgmInd].loop = true;
        if (fadeCurve != null)
        {
            StartCoroutine(FadeIn(fadeCurve));
        }
        else
        {
            bgm[bgmInd].Play();
        }
    }

    /// <summary>
    /// Fade in currently loaded BGM.
    /// </summary>
    /// <param name="fadeCurve">Volume curve.</param>
    IEnumerator FadeIn(AnimationCurve fadeCurve)
    {
        bgm[bgmInd].volume = 0f;
        bgm[bgmInd].Play();
        float time = 0f;
        while (time < 1f)
        {
            bgm[bgmInd].volume = fadeCurve.Evaluate(time);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Stop currently playing BGM (resets to beginning).
    /// </summary>
    /// <param name="fadeCurve">Volume curve.</param>
    public void StopBGM(AnimationCurve fadeCurve = null)
    {
        if (fadeCurve != null)
        {
            StartCoroutine(FadeOut(fadeCurve));
        }
        else
        {
            bgm[bgmInd].Stop();
        }
    }

    /// <summary>
    /// Fade out currently playing BGM.
    /// </summary>
    /// <param name="fadeCurve">Volume curve.</param>
    IEnumerator FadeOut(AnimationCurve fadeCurve)
    {
        float time = 0f;
        while (time < 1f)
        {
            bgm[bgmInd].volume = fadeCurve.Evaluate(time);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        bgm[bgmInd].Stop();
    }

    /// <summary>
    /// Pause/Unpause currently playing clip.
    /// </summary>
    /// <param name="pause">True: pause, false: unpause.</param>
    public void PauseBGM(bool pause)
    {
        if (pause) bgm[bgmInd].Pause();
        else bgm[bgmInd].UnPause();
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
