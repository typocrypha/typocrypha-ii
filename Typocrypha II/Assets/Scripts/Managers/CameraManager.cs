﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages camera effects.
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null; // Global static instance.
    public Vector3 basePos = new Vector3(0f, 0f, -10f); // Default camera position.
    public Transform cameraTr; // Transform of the camera.
    public GameObject faderPrefab; // Prefab of fader plane.

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

    /// <summary>
    /// Reset camera properties.
    /// </summary>
    public void ResetCamera()
    {
        StopAllCoroutines();
        cameraTr.position = basePos;
    }

    /// <summary>
    /// Shakes camera for some amount of time.
    /// </summary>
    /// <param name="intensity">Amount of shaking.</param>
    /// <param name="length">Length of shaking in seconds.</param>
    /// <param name="damp">[>0]: Amount of dampening (0 is no dampening).</param>
    public Coroutine Shake(float intensity, float length, float damp = 1f)
    {
        return StartCoroutine(_Shake(intensity, length, damp));
    }

    /// <summary>
    /// Shake camera once (just shifts randomly).
    /// </summary>
    /// <param name="intensity">Amount of shaking.</param>
    public void Shake(float intensity)
    {
        cameraTr.position = basePos + (Vector3)(Random.insideUnitCircle * intensity);
    }

    // Coroutine for shaking
    IEnumerator _Shake(float intensity, float length, float damp)
    {
        float endTime = Time.time + length;
        while (Time.time < endTime)
        {
            float ratio = Mathf.Pow((endTime - Time.time) / length, damp);
            Shake(intensity * ratio);
            yield return null;
        }
        cameraTr.position = basePos;
    }
}
