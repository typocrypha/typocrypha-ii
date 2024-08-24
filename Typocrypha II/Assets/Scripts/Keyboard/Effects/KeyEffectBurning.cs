using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Typocrypha
{
    /// <summary>
    /// Burning key effect.
    /// Affected key's output is doubled.
    /// </summary>
    public class KeyEffectBurning : KeyEffect
    {
        const float time = 12f; // Duration of fire.

        int reps = 0; // Number of additional times key is repeated.

        [SerializeField] private Image radialFill;

        public override void OnStart()
        {
            base.OnStart();
            // Multiply output.
            reps = 1;
            for (int i = 0; i < reps; i++)
            {
                key.AddToOutput(key.Letter);
            }
            // Start timer.
            StartCoroutine(DestroyAfterTime(time));
        }

        public override void OnPress() { }

        // Remove effect when time runs out.
        IEnumerator DestroyAfterTime(float seconds)
        {
            float curr = 0;
            while(curr < seconds)
            {
                if (PH.Paused)
                {
                    yield return new WaitWhile(PH.IsPaused);
                }
                yield return new WaitForEndOfFrame();
                curr += Time.deltaTime / Settings.GameplaySpeed;
                radialFill.fillAmount = 1 - Mathf.Min(curr / seconds, 1);
                if (PH.Paused)
                {
                    yield return new WaitWhile(PH.IsPaused);
                }
            }
            Remove();
        }
    }
}

