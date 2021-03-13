using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Frozen key effect.
    /// Affected key cannot be typed until key is mashed some number of times.
    /// </summary>
    public class KeyEffectFrozen : KeyEffect
    {
        int health = 0;

        const int minHealth = 3;
        const int maxHealth = 5;

        public override void OnStart()
        {
            key.output = ""; // Block output.
            health = Random.Range(minHealth, maxHealth);
            //StartCoroutine(DestroyAfterTime(4f)); // SHOULD KEY MELT NATURALLY?
        }

        public override void OnPress()
        {
            if (--health <= 0) Remove();
        }

        public override void Reset()
        {
            key.output = key.letter.ToString(); // Unblock output.
            key.onPress -= OnPress;
        }

        // Remove effect when time runs out.
        IEnumerator DestroyAfterTime(float seconds)
        {
            yield return new WaitForSecondsPause(seconds / Settings.GameplaySpeed, PH);
            Remove();
        }
    }
}

