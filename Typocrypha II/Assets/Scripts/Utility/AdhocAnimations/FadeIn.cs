using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhocAnim
{
    /// <summary>
    /// Fades in all listed sprite renderers on start.
    /// </summary>
    public class FadeIn : MonoBehaviour
    {
        public List<SpriteRenderer> srList;

        void Start()
        {
            StartCoroutine(FadeInCR());
        }

        IEnumerator FadeInCR()
        {
            float t = 0;
            while (t < 1f)
            {
                foreach(var sr in srList)
                {
                    sr.color = Color.white * t;
                }
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}

