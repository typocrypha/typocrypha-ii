using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhocAnim
{
    /// <summary>
    /// Linearly interpolate position to target position.
    /// </summary>
    public class Lerp : AdhocAnimation
    {
        public Vector3 targetPos; // Position to lerp to.
        public float speed; // Speed of interpolation (world units per fixed frame).

        public override void Play()
        {
            StartCoroutine(LerpCR());
        }

        public override void Stop()
        {
            StopAllCoroutines();
        }

        IEnumerator LerpCR()
        {
            var p = transform.position;
            var d = Vector3.Distance(transform.position, targetPos);
            var step = speed / d;
            for (var t = 0f; t < 1f; t += step)
            {
                transform.position = Vector3.Lerp(p, targetPos, t);
                yield return new WaitForFixedUpdate();
            }
            transform.position = targetPos;
        }
    }
}

