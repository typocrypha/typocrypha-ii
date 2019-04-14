using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdhocAnim
{
    /// <summary>
    /// Fades in/out all listed sprite renderers/images.
    /// </summary>
    public class Fade : AdhocAnimation
    {
        public bool fadeIn; // True: fade in. False: fade out.
        public float fadeTime = 1f; // Time it takes to completely fade. 
        public List<SpriteRenderer> srList;
        public List<Image> imageList;

        public override void Play()
        {
            StartCoroutine(FadeInCR());
        }

        public override void Stop()
        {
            StopAllCoroutines();
        }

        IEnumerator FadeInCR()
        {
            float t = 0;
            while (t < fadeTime)
            {
                float ratio = t / fadeTime;
                foreach(var sr in srList)
                {
                    if (fadeIn) sr.color = Color.white * ratio;
                    else sr.color = Color.white * (1f - ratio);
                }
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}

