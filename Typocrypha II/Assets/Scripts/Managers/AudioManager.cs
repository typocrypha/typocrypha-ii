using UnityEngine;
using System;
using System.IO;
using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine.Audio;
// using UnityEngine.SceneManagement;

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
    [SerializeField] private AudioSource[] textBlips; // Audio sources for playing text blip sfx. Number or sources should be divisible by 2

    AssetBundle sfxBundle; // Asset bundle containing sfx clips.
    int bgmInd; // Index of in use bgm audio source.
    private Coroutine routineFadeIn;
    private Coroutine routineFadeOut;

    public float BGMVolume
    {
        get => bgm[bgmInd].volume;
        set => bgm[bgmInd].volume = value;
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

        sfxBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sfx"));
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
        if (fadeCurve != null && fadeCurve.keys.Length > 0)
        {
            if(routineFadeIn != null)
            {
                StopCoroutine(routineFadeIn);
            }
            routineFadeIn = StartCoroutine(FadeIn(fadeCurve, bgmInd));
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
    /// <param name="incoming">Index of bgm to fade in.</param>
    IEnumerator FadeIn(AnimationCurve fadeCurve, int incoming)
    {
        bgm[incoming].volume = 0f;
        bgm[incoming].Play();
        float time = 0f;
        float length = fadeCurve.keys[fadeCurve.length - 1].time;
        while (time < length)
        {
            bgm[incoming].volume = fadeCurve.Evaluate(time);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        bgm[incoming].volume = 1f;
        routineFadeIn = null;
    }

    /// <summary>
    /// Stop currently playing BGM (resets to beginning).
    /// </summary>
    /// <param name="fadeCurve">Volume curve.</param>
    public void StopBGM(AnimationCurve fadeCurve = null)
    {
        if (fadeCurve != null && fadeCurve.keys.Length > 0)
        {
            if (routineFadeOut != null)
            {
                StopCoroutine(routineFadeOut);
            }
            routineFadeOut = StartCoroutine(FadeOut(fadeCurve, bgmInd));
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
    /// <param name="outgoing">Index of bgm to fade out.</param>
    IEnumerator FadeOut(AnimationCurve fadeCurve, int outgoing)
    {
        float time = 0f;
        float length = fadeCurve.keys[fadeCurve.length - 1].time;
        while (time < length)
        {
            bgm[outgoing].volume = fadeCurve.Evaluate(time);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        bgm[outgoing].Stop();
        routineFadeOut = null;
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
    /// Starts playing audio clip, crossfading over previous one.
    /// </summary>
    /// <param name="clip">Clip to play as bgm.</param>
    /// <param name="fadeCurveIn">Volume curve to fade in with.</param>
    /// <param name="fadeCurveOut">Volume curve to fade out with.</param>
    public void CrossfadeBGM(AudioClip clip, AnimationCurve fadeCurveIn = null, AnimationCurve fadeCurveOut = null)
    {
        StopBGM(fadeCurveOut);
        bgmInd = 1 - bgmInd; //switch active track
        PlayBGM(clip, fadeCurveIn);
    }

    /// <summary>
    /// Plays an sfx once.
    /// </summary>
    /// <param name="clip">Clip to play.</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;
        sfx.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays an sfx once.
    /// </summary>
    /// <param name="clipName">Name of clip in sfx asset bundle to play.</param>
    public void PlaySFX(String clipName)
    {
        if (String.IsNullOrEmpty(clipName)) return;

        var clip = sfxBundle.LoadAsset<AudioClip>(clipName);

        if (clip != null) sfx.PlayOneShot(clip);
        else Debug.LogWarning($"Clip named \"{clipName}\" not found.");
    }

    public void PlayTextScrollSfx(AudioClip clip)
    {
        foreach(var source in textBlips)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(clip);
                return;
            }
        }
        textBlips[0].PlayOneShot(clip);
    }
}
