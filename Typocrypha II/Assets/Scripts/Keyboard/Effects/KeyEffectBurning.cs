﻿using System.Collections;
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

        int reps = 0; // Number of additional times key is repeated.

        public override void OnStart()
        {
            base.OnStart();
            // Multiply output.
            reps = Random.Range(1, 4);
            for (int i = 0; i < reps; i++)
            {
                key.AddToOutput(key.letter);
            }
            // Start timer.
            StartCoroutine(DestroyAfterTime(time));
        }

        public override void OnPress()
        {
            
        }

        public override void Reset()
        {
            base.Reset();
            // Revert output.
            key.SetOutput(key.letter);
        }

        // Remove effect when time runs out.
        IEnumerator DestroyAfterTime(float seconds)
        {
            yield return new WaitForSecondsPause(seconds / Settings.GameplaySpeed, PH);
            Remove();
        }
    }
}

