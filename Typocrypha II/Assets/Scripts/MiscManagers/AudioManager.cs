using UnityEngine;
using System;
using System.IO;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
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
    [SerializeField] private StudioEventEmitter bgmEmitter; // FMOD event emitter for bgm
    [SerializeField] private AudioSource[] bgm; // Audio sources for playing bgms. Should have 2 audio sources (for crossfading).
    [SerializeField] private AudioSource sfx; // Audio source for playing simple sfx.
    [SerializeField] private AudioSource[] textBlips; // Audio sources for playing text blip sfx. Number or sources should be divisible by 2
    [SerializeField] AudioClipBundle sfxBundle; // Better bundle containing sfx clips.
    [SerializeField] private FMODEvents eventManager; // FMOD event reference utility

    int bgmInd; // Index of in use bgm audio source.
    private Coroutine routineFadeIn;
    private Coroutine routineFadeOut;
    private EventInstance eventInstance;

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
            DontDestroyOnLoad(gameObject);
            bgmInd = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(bgmEmitter.EventReference);
        eventInstance.start();
        bgmEmitter.Play();
    }

    private void StopFades()
    {
        if (routineFadeOut != null)
        {
            StopCoroutine(routineFadeOut);
            routineFadeOut = null;
        }
        if (routineFadeIn != null)
        {
            StopCoroutine(routineFadeIn);
            routineFadeIn = null;
        }
    }

    private float ScaleToFMOD(float seconds)
    {
        if (seconds < 0f || seconds > 10f) throw new ArgumentOutOfRangeException();
        return (seconds <= 1f) ? seconds : (seconds / 10f) + 1f;
    }

    public void PlayBGMEvent(string FMODEventPath, float attack = 0f)
    {
        eventInstance.setParameterByName("Attack", ScaleToFMOD(attack));
        eventInstance.setParameterByName("Release", ScaleToFMOD(0f));
        eventInstance.setParameterByNameWithLabel("BGM", FMODEventPath);
        eventInstance.getPlaybackState(out var state);
        if (state != PLAYBACK_STATE.PLAYING) eventInstance.start();
    }

    public void StopBGMEvent(float release = 0f)
    {
        eventInstance.setParameterByName("Release", ScaleToFMOD(release));
        eventInstance.setParameterByName("BGM", 0);
    }

    /// <summary>
    /// Starts playing audio clip from beginning.
    /// </summary>
    /// <param name="clip">Clip to play as bgm.</param>
    /// <param name="fadeCurve">Volume curve to fade in with.</param>
    public void PlayBGM(AudioClip clip, AnimationCurve fadeCurve = null)
    {
        if (bgm[bgmInd].clip == clip && bgm[bgmInd].isPlaying)
            return;
        StopFades();
        PlayBGMInternal(clip, fadeCurve);
    }

    private void PlayBGMInternal(AudioClip clip, AnimationCurve fadeCurve)
    {
        bgm[bgmInd].clip = clip;
        bgm[bgmInd].loop = true;
        if (fadeCurve != null && fadeCurve.keys.Length > 0)
        {
            routineFadeIn = StartCoroutine(FadeIn(fadeCurve, bgmInd));
        }
        else
        {
            bgm[bgmInd].volume = 1f;
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
        StopFades();
        StopBGMInternal(fadeCurve);
    }

    private void StopBGMInternal(AnimationCurve fadeCurve)
    {
        if (fadeCurve != null && fadeCurve.keys.Length > 0)
        {
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

    public void PauseBGMEvent(bool pause)
    {
        eventInstance.setPaused(pause);
    }

    /// <summary>
    /// Starts playing audio clip, crossfading over previous one.
    /// </summary>
    /// <param name="clip">Clip to play as bgm.</param>
    /// <param name="fadeCurveIn">Volume curve to fade in with.</param>
    /// <param name="fadeCurveOut">Volume curve to fade out with.</param>
    public void CrossfadeBGM(AudioClip clip, AnimationCurve fadeCurveIn = null, AnimationCurve fadeCurveOut = null)
    {
        if (bgm[bgmInd].clip == clip && bgm[bgmInd].isPlaying)
            return;
        StopFades();
        StopBGMInternal(fadeCurveOut);
        bgmInd = 1 - bgmInd; //switch active track
        PlayBGMInternal(clip, fadeCurveIn);
    }

    public void CrossfadeBGMEvent(string FMODEventPath, float attack = 1f, float release = 1f)
    {
        eventInstance.setParameterByName("Attack", ScaleToFMOD(attack));
        eventInstance.setParameterByName("Release", ScaleToFMOD(release));
        eventInstance.setParameterByNameWithLabel("BGM", FMODEventPath);
    }

    /// <summary>
    /// Plays an sfx once.
    /// </summary>
    /// <param name="clip">Clip to play.</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        PlaySFX(FMODEvents.ClipToRef(clip));
    }

    public void PlaySFX(EventReference reference)
    {
        RuntimeManager.PlayOneShot(reference);
    }

    /// <summary>
    /// Plays an sfx once.
    /// </summary>
    /// <param name="clipName">Name of clip in sfx asset bundle to play.</param>
    public void PlaySFX(string clipName)
    {
        PlaySFX(FMODEvents.NameToRef(clipName));
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
