using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public Camera Camera => camera;
    [SerializeField] private new Camera camera;

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

    private Bounds ReduceBoundsByCameraSize(Bounds bounds)
    {
        bounds.extents = new Vector3(
            Mathf.Max(0, bounds.extents.x - Camera.orthographicSize * Camera.aspect),
            Mathf.Max(0, bounds.extents.y - Camera.orthographicSize),
            bounds.extents.z);
        return bounds;
    }

    private Vector3 GetPositionWithinBounds(Bounds bounds, Vector2 pivot)
    {
        //constrain x and y between 0 and 1
        Bounds pivotBounds = new Bounds(Vector2.one/2, Vector2.one);
        pivot = pivotBounds.ClosestPoint(pivot);

        var cameraBounds = ReduceBoundsByCameraSize(bounds);
        var xPos = Mathf.Lerp(cameraBounds.min.x, cameraBounds.max.x, pivot.x);
        var yPos = Mathf.Lerp(cameraBounds.min.y, cameraBounds.max.y, pivot.y);
        return new Vector3(xPos, yPos, cameraTr.position.z);
    }

    public Tween MoveToPivot(Bounds bounds, Vector2 pivotStart, Vector2 pivotFinal, float duration, AnimationCurve ease)
    {
        var start = GetPositionWithinBounds(bounds, pivotStart);
        var final = GetPositionWithinBounds(bounds, pivotFinal);
        Debug.LogFormat("Start: {0}, Final: {1}", start, final);
        return cameraTr.DOMove(final, duration).From(start).SetEase(ease);
    }
}
