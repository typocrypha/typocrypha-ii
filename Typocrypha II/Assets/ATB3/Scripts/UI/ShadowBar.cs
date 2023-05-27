using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Delayed bar value change effect (e.g. for health bars).
/// </summary>
public class ShadowBar : MonoBehaviour
{
    public Image shadow; // Slider for bar
    float curr; // Normalized current amount
    public float Curr
    {
        get => curr;
        set
        {
            if (value == curr)
                return;
            if (value > curr || !gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                shadow.fillAmount = value;
            }
            else
            {
                StartCoroutine(Transition(shadow, shadowDelay, shadowTime, value));
            }
            curr = value;
        }
    }
    public float shadowDelay = 0.5f; // Delay before shadow changes
    public float shadowTime = 0.5f; // Time it takes for shadow to reach target amount

    IEnumerator Transition(Image bar, float delay, float time, float target)
    {
        if (delay != 0f)
            yield return new WaitForSeconds(delay);
        float steps = Mathf.Floor(time / Time.fixedDeltaTime);
        float start = bar.fillAmount;
        if (time != 0f)
        {
            for (float step = 0; step < steps; step++)
            {
                float scale = Mathf.Lerp(start, target, step / steps);
                bar.fillAmount = scale;
                yield return new WaitForFixedUpdate();
            }
        }
        bar.fillAmount = target;
    }
}

