using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Burning key effect.
    /// Affected key's output is doubled.
    /// </summary>
    public class KeyEffectBurning : KeyEffect
    {
        const float time = 5f; // Duration of fire.

        public override void OnStart()
        {
            // Double output.
            key.output = key.output + key.output;
            // Start timer.
            StartCoroutine(DestroyAfterTime(time));
        }

        public override void OnPress()
        {
            
        }

        public override void Reset()
        {
            // Revert output.
            key.output = key.letter.ToString();
            key.onPress -= OnPress;
        }

        // Remove effect when time runs out.
        IEnumerator DestroyAfterTime(float seconds)
        {
            yield return new WaitForSecondsPause(seconds, PH);
            Remove();
        }
    }
}

