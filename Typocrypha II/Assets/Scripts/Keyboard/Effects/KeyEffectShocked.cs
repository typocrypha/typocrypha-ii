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

        const float time = 15f; // Duration of shock.

        public GameObject swappedKeyEffectPrefab;

        private GameObject swappedKeyEffect;
        private char swappedWith = ' ';

        public override void OnStart()
        {
            base.OnStart();
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
            key.SetOutputAndDisplay(swappedWith);
            // Set the output of the key we swapped with
            Key swappedKey = Keyboard.instance.keyMap[swappedWith];
            swappedKey.SetOutputAndDisplay(key.Letter);
            swappedKey.SfxOverride = sfxOverride;
            // Create the visuals for the other key
            swappedKeyEffect = Instantiate(swappedKeyEffectPrefab, swappedKey.KeyEffectContainer);

            // Start timer.
            StartCoroutine(DestroyAfterTime(time));
        }

        public override void OnPress()
        {
            
        }

        public override void ResetEffect()
        {
            base.ResetEffect();
            // Revert swapped key output
            Key swappedKey = Keyboard.instance.keyMap[swappedWith];
            swappedKey.SetOutputAndDisplay(swappedWith);
            swappedKey.SfxOverride = null;
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

