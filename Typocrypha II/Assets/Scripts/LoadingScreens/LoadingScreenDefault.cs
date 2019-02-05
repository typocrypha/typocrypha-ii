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

    Animator animator; // Loading screen animator

    void Awake()
    {
        loadingPercent.text = 0f.ToString() + "%";
        animator = GetComponent<Animator>();
    }

    public override float Progress
    {
        set
        {
            loadingPercent.text = (value * 100).ToString() + "%";
            // When done loading, start fading animation. Animator handles destruction.
            if (value == 1.0f)
            {
                animator.SetBool("DoneLoading", true);
                StartCoroutine(CheckFading());
            }
        }
    }

    // When done fading, set Done.
    IEnumerator CheckFading()
    {
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < .9f);
        _done = true;
    }

    // Done is set when loading screen starts fading.
    bool _done = false;
    public override bool Done
    {
        get
        {
            return _done;
        }
    }
}
