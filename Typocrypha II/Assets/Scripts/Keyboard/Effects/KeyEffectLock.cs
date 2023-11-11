using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Burning key effect.
    /// Affected key's output is doubled.
    /// </summary>
    public class KeyEffectLock : KeyEffect
    {
        public override void OnStart()
        {
            base.OnStart();
            if(caster == null)
            {
                Remove();
                return;
            }
            key.ClearOutput();
            caster.OnSpiritMode -= Remove;
            caster.OnSpiritMode += Remove;
        }

        public override void OnPress() 
        {
            if(sfxOverride != null)
            {
                AudioManager.instance.PlaySFX(sfxOverride);
            }
        }
    }
}

