using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Default loading screen.
/// </summary>
[RequireComponent(typeof(Animator))]
public class LoadingScreenDefault : LoadingScreen
{
    public TextMeshPro loadingPercent; // Text display of loading percentage

    void Awake()
    {
        loadingPercent.text = 0f.ToString() + "%";
        animator = GetComponent<Animator>();
        StartCoroutine(CheckFading());
    }

    public override float Progress
    {
        set
        {
            loadingPercent.text = (value * 100).ToString() + "%";
            // When done loading, start fading out animation. Animator handles destruction.
            if (value == 1.0f)
            {
                _done = true;
            }
        }
    }

    // When done fading in, set Done.
    IEnumerator CheckFading()
    {
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < .9f);
        _ready = true;
    }

    // Ready is set when loading screen faded in.
    bool _ready = false;
    public override bool ReadyToLoad
    {
        get
        {
            return _ready;
        }
    }

    // Done is set when scene is loaded.
    bool _done = false;
    public override bool DoneLoading
    {
        get
        {
            return _done;
        }
    }
}
