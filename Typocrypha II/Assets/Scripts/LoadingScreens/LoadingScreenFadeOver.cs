using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fade in next background.
/// </summary>
public class LoadingScreenFadeOver : LoadingScreenDefault
{
    public float _Alpha;
    public float Alpha // Amount of Fade
    {
        set
        {
            foreach (SpriteRenderer sr in BackgroundManager.instance.bggo.GetComponents<SpriteRenderer>())
            {
                sr.SetAlpha(value);
            }
        }
    }

    void Start()
    {
        _Alpha = 1f;
    }

    void Update()
    {
        Alpha = _Alpha;
    }
}
