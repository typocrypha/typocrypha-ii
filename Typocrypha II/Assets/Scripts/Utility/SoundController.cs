using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    private List<AudioSource> sfxList = new List<AudioSource>();
    private const int SFX_LIMIT = 16;
    private bool isPaused = false;

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
    }

    public AudioSource PlaySingle(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return null;
        Clear();
        AudioSource sfx = gameObject.AddComponent<AudioSource>() as AudioSource;
        
        sfxList.Add(sfx);

        if (sfxList.Count > SFX_LIMIT)
        {
            Destroy(sfxList[0]);
            sfxList.RemoveAt(0);
        }

        sfx.clip = clip;
        sfx.volume = volume;
        sfx.Play();

        return sfx;
    }

    public void PauseAll()
    {
        Clear();
        isPaused = true;
        foreach (AudioSource sfx in sfxList) sfx.Pause();
    }

    public void ResumeAll()
    {
        Clear();
        isPaused = false;
        foreach (AudioSource sfx in sfxList) sfx.Play();
    }

    public void StopAll()
    {
        Clear();
        isPaused = false;
        foreach (AudioSource sfx in sfxList) sfx.Stop();
    }

    public void Clear()
    {
        var toDelete = new List<AudioSource>();
        for (int i = 0; i < sfxList.Count; ++i)
        {
            var sfx = sfxList[i];
            if (!isPaused && !sfx.isPlaying)
            {
                sfxList.RemoveAt(i);
                Destroy(sfx);
            }
        }
    }
}
