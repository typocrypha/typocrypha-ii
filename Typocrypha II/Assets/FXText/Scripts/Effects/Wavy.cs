using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Makes text wave up and down in a sin wave
    public class Wavy : FXTextBase
    {
        public float amplitude;
        public float waveLength;
        public float frequency;
        float step;

        protected override void onEffect(VertexHelper vh, int pos)
        {
            int start = pos * 4;
            float height = 0f;
            if (waveLength > 0)
                height = Mathf.Sin((step * frequency) + (pos / waveLength)) * amplitude;
            for (int k = start; k < start + 4; k++)
            {
                UIVertex vert = new UIVertex();
                vh.PopulateUIVertex(ref vert, k);
                vert.position += Vector3.up * height;
                vh.SetUIVertex(vert, k);
            }
        }

        protected override IEnumerator effectCR()
        {
            while(true)
            {
                step += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}

