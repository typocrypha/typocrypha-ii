using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Delayed bar value change effect (e.g. for health bars).
/// </summary>
public class ShadowBar : MonoBehaviour
{
    public Slider shadow; // Slider for bar
    float _curr; // Normalized current amount
    public float Curr
    {
        get
        {
            return _curr;
        }
        set
        {
            _curr = value;
            StartCoroutine(Transition(shadow, shadowDelay, shadowTime, Curr));
        }
    }
    public float shadowDelay = 0.5f; // Delay before shadow changes
    public float shadowTime = 0.5f; // Time it takes for shadow to reach target amount

    IEnumerator Transition(Slider bar, float delay, float time, float target)
    {
        if (delay != 0f)
            yield return new WaitForSeconds(delay);
        float steps = Mathf.Floor(time / Time.fixedDeltaTime);
        float start = bar.value;
        if (time != 0f)
        {
            for (float step = 0; step < steps; step++)
            {
                float scale = Mathf.Lerp(start, target, step / steps);
                bar.value = scale;
                yield return new WaitForFixedUpdate();
            }
        }
        bar.value = target;
    }
    /// <summary>
    /// Reset shadow to 0 without transitions (immediate)
    /// </summary>
    public void Reset()
    {
        shadow.value = 0;
    }
}

