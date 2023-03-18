using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages camera effects.
/// </summary>
public class CameraManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
    }
    #endregion
    public static CameraManager instance = null; // Global static instance.
    public Vector3 basePos = new Vector3(0f, 0f, -10f); // Default camera position.
    public Transform cameraTr; // Transform of the camera.

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

        ph = new PauseHandle(OnPause);
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
    public Coroutine Shake(float intensity, float length, float damp = 3f)
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
        float time = 0;
        while (time < length)
        {
            if (this.IsPaused())
            {
                yield return new WaitWhile(this.IsPaused);
            }
            float ratio = Mathf.Pow((length - time) / length, damp);
            Shake(intensity * ratio);
            yield return new WaitForSeconds(0.01f);
            time += 0.01f;
        }
        cameraTr.position = basePos;
    }
}
