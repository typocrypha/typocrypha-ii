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

        [SerializeField] private AudioClip breakSfx;

        public override void OnStart()
        {
            base.OnStart();
            key.ClearOutput();
            health = Random.Range(minHealth, maxHealth);
        }

        public override void OnPress()
        {
            if (--health <= 0) Remove();
            if(health == 1)
            {
                key.SfxOverride = breakSfx;
            }
        }

        public override void Reset()
        {
            base.Reset();
            key.SetOutput(key.letter); // Unblock output.
        }
    }
}

