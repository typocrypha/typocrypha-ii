using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Default loading screen. Fades in and out to hide loading.
/// </summary>
public class LoadingScreenDefault : LoadingScreen
{
    public const float fadeTime = 2f;
    public const float fadeTimeStaggered = fadeTime * 0.75f;

    public override float Progress
    {
        set
        {
        }
    }

    public override Coroutine StartLoading()
    {
        if (FaderManager.instance.ScreenFadeColor == Color.black)
        {
            return null;
        }
        else
        {
            return StartCoroutine(StartLoadingCr());
        }
    }

    private IEnumerator StartLoadingCr()
    {
        yield return FaderManager.instance.FadeScreenOverTime(fadeTime, FaderManager.instance.ScreenFadeColor.a, 1, Color.black, false);
    }

    public override Coroutine FinishLoading()
    {
        if (FaderManager.instance.IsFadingScreen)
        {
            return StartCoroutine(WaitUntilScreenFadeIsComplete());
        }
        FaderManager.instance.FadeScreenOverTime(fadeTime, 1, 0, Color.black, false);
        return null;
    }

    private IEnumerator WaitUntilScreenFadeIsComplete()
    {
        yield return new WaitWhile(() => FaderManager.instance.IsFadingScreen);
    }
}
