using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Default loading screen. Fades in and out to hide loading.
/// </summary>
public class LoadingScreenBumper : LoadingScreen
{
    public float fadeInTime = 1f;
    public float fadeInBufferTime = 1f;
    public float holdTime = 3f;
    public float fadeOutTime = 1f;
    [SerializeField] private TextMeshProUGUI catchPhrase; // Text display of catchy phrase
    [SerializeField] private UnityEngine.UI.Image bumperImage;
    [SerializeField] private AudioSource bumperAudio;

    void Awake()
    {
        
    }

    public override float Progress
    {
        set
        {
            
        }
    }

    public override Coroutine StartLoading()
    {
        return StartCoroutine(StartLoadingCr());
    }

    private IEnumerator StartLoadingCr()
    {
        if (FaderManager.instance.ScreenFadeColor != Color.black)
        {
            yield return FaderManager.instance.FadeScreenOverTime(fadeInTime, FaderManager.instance.ScreenFadeColor.a, 1, Color.black, false);
        }
        yield return new WaitForSeconds(fadeInBufferTime);
        bumperAudio.Play();
        catchPhrase.gameObject.SetActive(true);
        bumperImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(holdTime);
    }

    public override Coroutine FinishLoading()
    {
        if (FaderManager.instance.IsFadingScreen)
        {
            return StartCoroutine(WaitUntilScreenFadeIsComplete());
        }
        FaderManager.instance.FadeScreenOverTime(fadeOutTime, 1, 0, Color.black, false);
        return null;
    }

    private IEnumerator WaitUntilScreenFadeIsComplete()
    {
        yield return new WaitWhile(() => FaderManager.instance.IsFadingScreen);
        catchPhrase.gameObject.SetActive(false);
        bumperImage.gameObject.SetActive(false);
    }
}
