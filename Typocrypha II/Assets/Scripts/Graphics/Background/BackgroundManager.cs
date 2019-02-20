using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the background and background effects.
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance = null;
    public SpriteRenderer bgsr; // Sprite renderer for the background.

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
}
