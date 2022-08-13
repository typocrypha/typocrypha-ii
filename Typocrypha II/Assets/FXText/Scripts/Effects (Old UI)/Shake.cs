using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Makes text wave up and down in a sin wave
    public class Shake : FXTextBase
    {
        public float intensity = 1f;
        Vector3[] offset;
        int step;

        const int randSize = 16;

        protected override void OnEffect(List<UIVertex> verts, int pos)
        {
            UIVertex vt;
            for (int i = 0; i < verts.Count; i++)
            {
                vt = verts[i];
                vt.position += offset[(pos + step) % randSize];
                verts[i] = vt;
            }
        }

        protected override IEnumerator EffectCR()
        {
            offset = new Vector3[randSize];
            for (int i = 0; i < randSize; i++)
            {
                offset[i] = (Vector3)(Random.insideUnitCircle * intensity);
            }
            while (true)
            {
                step++;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}


