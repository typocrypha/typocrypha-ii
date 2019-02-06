using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages camera effects.
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null; // Global static instance
    public Transform cameraTr; // Transform of the camera
    public GameObject faderPrefab; // Prefab of fader plane

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
    /// Shakes camera.
    /// </summary>
    /// <param name="intensity">Amount of shaking.</param>
    /// <param name="length">Length of shaking in seconds.</param>
    public void Shake(float intensity, float length)
    {
        StartCoroutine(_Shake(intensity, length));
    }

    // Coroutine for shaking
    IEnumerator _Shake(float intensity, float length)
    {
        float endTime = Time.time + length;
        Vector3 oldPos = cameraTr.position;
        while (Time.time < endTime)
        {
            cameraTr.position = oldPos + (Vector3)Random.insideUnitCircle;
            yield return null;
        }
        cameraTr.position = oldPos;
    }
}
