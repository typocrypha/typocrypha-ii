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
        isPaused = true;
        foreach (AudioSource sfx in sfxList) sfx.Pause();
    }

    public void ResumeAll()
    {
        isPaused = false;
        foreach (AudioSource sfx in sfxList) sfx.Play();
    }

    public void StopAll()
    {
        isPaused = false;
        foreach (AudioSource sfx in sfxList) sfx.Stop();
    }

    void Update()
    {
        foreach (AudioSource sfx in sfxList)
        {
            if (!isPaused && !sfx.isPlaying)
            {
                sfxList.Remove(sfx);
                Destroy(sfx);
            }
        }
    }

}
