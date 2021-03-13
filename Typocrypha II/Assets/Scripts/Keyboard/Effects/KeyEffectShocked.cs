using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Shocked key effect.
    /// Affected key's output is swapped with another key.
    /// To apply, just apply to a single key. Then, that key will pick a random key to set.
    /// TODO: allow for manually setting shock pairs.
    /// </summary>
    public class KeyEffectShocked : KeyEffect
    {
        public override int NumAffectedKeys => 2;

        const float time = 5f; // Duration of shock.

        public GameObject swappedKeyEffectPrefab;

        private GameObject swappedKeyEffect;
        private char swappedWith = ' ';

        public override void OnStart()
        {
            // Pick random, unaffected key.
            swappedWith = Keyboard.instance.GetRandomUnaffectedKey();
            // If there are no other unaffected keys (this should never happen)
            if (swappedWith == Keyboard.randomKeyFail)
            {
                Remove();
                return;
            }
            // Mark the other key as being affected
            MarkAffected(swappedWith);
            // Swap self output.
            key.output = swappedWith.ToString();
            key.letterText.text = swappedWith.ToString().ToUpper();
            // Set the output of the key we swapped with
            Key swappedKey = Keyboard.instance.keyMap[swappedWith];
            swappedKey.output = key.letter.ToString();
            swappedKey.letterText.text = key.letter.ToString().ToUpper();
            // Create the visuals for the other key
            swappedKeyEffect = Instantiate(swappedKeyEffectPrefab, swappedKey.transform);

            // Start timer.
            StartCoroutine(DestroyAfterTime(time));
        }

        public override void OnPress()
        {
            
        }

        public override void Reset()
        {
            // Revert own output.
            key.output = key.letter.ToString();
            key.letterText.text = key.letter.ToString().ToUpper();
            key.onPress -= OnPress;
            // Revert swapped key output
            Key swappedKey = Keyboard.instance.keyMap[swappedWith];
            swappedKey.output = swappedWith.ToString();
            swappedKey.letterText.text = swappedWith.ToString().ToUpper();
            // Destroy Swapped key effect graphics
            Destroy(swappedKeyEffect);
            // Mark swapped key unaffected
            MarkUnaffected(swappedWith);
        }

        // Remove effect when time runs out.
        IEnumerator DestroyAfterTime(float seconds)
        {
            yield return new WaitForSecondsPause(seconds / Settings.GameplaySpeed, PH);
            Remove();
        }
    }
}

