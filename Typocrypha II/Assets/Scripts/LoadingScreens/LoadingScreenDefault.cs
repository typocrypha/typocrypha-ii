﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Default loading screen. Fades in and out to hide loading.
/// </summary>
public class LoadingScreenDefault : LoadingScreen
{
    private const float fadeTime = 2f;
    [SerializeField] private TextMeshProUGUI loadingPercent; // Text display of loading percentage

    void Awake()
    {
        loadingPercent.text = 0f.ToString() + "%";
    }

    public override float Progress
    {
        set
        {
            loadingPercent.text = Mathf.RoundToInt(value * 100).ToString() + "%";
        }
    }

    public override Coroutine StartLoading()
    {
        if (FaderManager.instance.ScreenFadeColor == Color.black)
        {
            loadingPercent.gameObject.SetActive(true);
            return null;
        }
        else
        {
            return StartCoroutine(StartLoadingCr());
        }
    }

    private IEnumerator StartLoadingCr()
    {
        yield return FaderManager.instance.FadeScreenOverTime(fadeTime, FaderManager.instance.ScreenFadeColor.a, 1, Color.black);
        loadingPercent.gameObject.SetActive(true);
    }

    public override Coroutine FinishLoading()
    {
        loadingPercent.gameObject.SetActive(false);
        if (FaderManager.instance.IsFadingScreen)
        {
            return StartCoroutine(WaitUntilScreenFadeIsComplete());
        }
        return FaderManager.instance.FadeScreenOverTime(fadeTime, 1, 0, Color.black);
    }

    private IEnumerator WaitUntilScreenFadeIsComplete()
    {
        yield return new WaitWhile(() => FaderManager.instance.IsFadingScreen);
    }
}
