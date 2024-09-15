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

        public override EffectType EffectID => EffectType.Frozen;

        public override void OnStart()
        {
            base.OnStart();
            key.ClearOutput();
            key.ForceSfx = true;
            int baseHealth = Random.Range(minHealth, maxHealth);
            if(PlayerDataManager.instance.equipment.TryGetEquippedBadgeEffect<BadgeEffectFrozenHitsModifier>(out var mod))
            {
                baseHealth += mod.Modifier;
            }
            if(baseHealth <= 0)
            {
                Remove();
                return;
            }
            health = baseHealth;
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

