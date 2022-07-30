using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Animate letter positions from simple animation curves. Can cascade the effect
    public class Cascade : FXTextBase
    {
        public AnimationCurve curveX; // Curve that controls x-offset
        public AnimationCurve curveY; // Curve that controls y-offset
        public float delay; // Time before movement in next letter
        float startTime; // Time effect started
        float time // Track amount of time passed
        {
            get
            {
                return Time.fixedTime - startTime;
            }
        }
        Vector3[] offsets; // All offsets of each character

        const int maxLength = 256; // Max number of characters

        protected override IEnumerator EffectCR()
        {
            startTime = Time.fixedTime;
            offsets = new Vector3[maxLength];
            // Start delayed update coroutines
            for (int i = 0; i < ind.Count; i += 2)
            {
                for (int j = ind[i]; j < ind[i + 1]; j++)
                {
                    StartCoroutine(delayed(j));
                }
            }
            yield return true;
        }

        // Updates the position delayed based on index
        IEnumerator delayed(int pos)
        {
            while (true)
            {
                offsets[pos].x = curveX.Evaluate(time - delay * pos);
                offsets[pos].y = curveY.Evaluate(time - delay * pos);
                yield return new WaitForFixedUpdate();
            }
        }

        protected override void OnEffect(List<UIVertex> verts, int pos)
        {
            UIVertex vt;
            for (int i = 0; i < verts.Count; i++)
            {
                vt = verts[i];
                vt.position += offsets[pos];
                verts[i] = vt;
            }
        }
    }
}

