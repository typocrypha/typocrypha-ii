using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Signals boss incoming and changes music.
    /// </summary>
    public class BossWarning : MonoBehaviour
    {
        public AudioSource bgm; // BGM player.
        public AudioClip bossTheme; // Boss bgm.
        public TMPro.TextMeshPro warningText;

        bool started = false;

        /// <summary>
        /// Check when entering field.
        /// </summary>
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (!started)
            {
                started = true;
                StartCoroutine(TimedEffects());
            }
        }

        IEnumerator TimedEffects()
        {
            yield return new WaitForSeconds(2f);
            bgm.clip = bossTheme;
            bgm.Play();
            bool flip = false;
            while(true)
            {
                yield return new WaitForSeconds(0.5f);
                if (flip = !flip)
                    warningText.color = Color.red;
                else
                    warningText.color = Color.white;
            }
        }
    }
}

