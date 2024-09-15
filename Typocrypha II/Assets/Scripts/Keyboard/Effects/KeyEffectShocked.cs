using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public override EffectType EffectID => EffectType.Shocked;
        public override int NumAffectedKeys => 2;

        const float time = 12f; // Duration of shock.

        public float FillAmount { get => radialFill.fillAmount; set => radialFill.fillAmount = value; }

        public GameObject swappedKeyEffectPrefab;
        [SerializeField] private Image radialFill;

        private GameObject swappedKeyEffect;
        private KeyEffectShockedSub swappedShockEffect;
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
            swappedShockEffect = swappedKeyEffect.GetComponent<KeyEffectShockedSub>();
            // Start timer
            var modTime = time;
            if (PlayerDataManager.instance.equipment.TryGetEquippedBadgeEffect<BadgeEffectShockedTimeMultiplier>(out var multiplier))
            {
                modTime *= multiplier.Multiplier;
            }
            if (modTime <= 0)
            {
                Remove();
                return;
            }
            StartCoroutine(DestroyAfterTime(modTime));
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
            float curr = 0;
            while (curr < seconds)
            {
                if (PH.Paused)
                {
                    yield return new WaitWhile(PH.IsPaused);
                }
                yield return new WaitForEndOfFrame();
                curr += Time.deltaTime / Settings.GameplaySpeed;
                radialFill.fillAmount = 1 - Mathf.Min(curr / seconds, 1);
                swappedShockEffect.FillAmount = radialFill.fillAmount;
                if (PH.Paused)
                {
                    yield return new WaitWhile(PH.IsPaused);
                }
            }
            Remove();
        }
    }
}

