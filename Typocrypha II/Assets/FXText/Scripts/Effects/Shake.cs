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

        const int scrambleSize = 16;

        protected override void onEffect(VertexHelper vh, int pos)
        {
            int start = pos * 4;
            for (int k = start; k < start + 4; k++)
            {
                UIVertex vert = new UIVertex();
                vh.PopulateUIVertex(ref vert, k);
                vert.position += offset[(pos + step) % scrambleSize];
                vh.SetUIVertex(vert, k);
            }
        }

        protected override IEnumerator effectCR()
        {
            offset = new Vector3[scrambleSize];
            for (int i = 0; i < scrambleSize; i++)
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


