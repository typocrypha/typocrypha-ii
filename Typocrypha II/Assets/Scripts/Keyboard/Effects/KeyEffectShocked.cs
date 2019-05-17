using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Shocked key effect.
    /// Affected key's output is swapped with another key.
    /// To apply, just apply to a single key. Then, that key will pick a random key to set.
    /// TODO: allow for manually setting shock pairs (probably through swapped map).
    /// </summary>
    public class KeyEffectShocked : KeyEffect
    {
        public GameObject effectPrefab; // For application of effect on swapped key.

        static Dictionary<char, char> swapped = new Dictionary<char, char>(); // All pairs of swapped keys.

        const float time = 5f; // Duration of shock.

        public override void OnStart()
        {
            // If self is not already swapped (i.e. the 'starter' key).
            if (!swapped.ContainsKey(key.letter) || swapped[key.letter] == key.letter)
            {
                // Pick random, unaffected key.
                char c = key.letter;
                while (c == key.letter && Keyboard.instance.keyMap[c].Affected)
                    c = (char)('a' + Random.Range(0, 26));
                // Set map.
                swapped[key.letter] = c;
                swapped[c] = key.letter;
                // Swap self output.
                key.output = c.ToString();
                key.letterText.text = c.ToString().ToUpper();
                // Apply effect to other key;
                Keyboard.instance.keyMap[c].ApplyEffect(effectPrefab);
            }
            else // Otherwise, we've already been paired (i.e. the 'other' key)
            {
                // Swap self output.
                key.output = swapped[key.letter].ToString();
                key.letterText.text = swapped[key.letter].ToString().ToUpper();
            }
            // Start timer.
            StartCoroutine(DestroyAfterTime(time));
        }

        public override void OnPress()
        {
            
        }

        public override void Reset()
        {
            // Reset map.
            swapped[key.letter] = key.letter;
            // Revert output.
            key.output = key.letter.ToString();
            key.letterText.text = key.letter.ToString().ToUpper();
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

