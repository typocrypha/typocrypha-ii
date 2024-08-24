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

        const int minHealth = 2;
        const int maxHealth = 2;

        [SerializeField] private AudioClip breakSfx;


        public override void OnStart()
        {
            base.OnStart();
            key.ClearOutput();
            key.ForceSfx = true;
            health = Random.Range(minHealth, maxHealth);
        }

        public override void ResetEffect()
        {
            base.ResetEffect();
            key.ForceSfx = false;
        }

        public override void OnPress()
        {
            if (--health <= 0)
            {
                AudioManager.instance.PlaySFX(breakSfx);
                Remove();
                return;
            }
        }
    }
}

