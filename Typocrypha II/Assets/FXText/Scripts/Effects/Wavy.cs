using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Makes text wave up and down in a sin wave
    public class Wavy : FXTextBase
    {
        public float amplitude = 1f;
        public float waveLength = 1f;
        public float frequency = 6f;
        float step;

        protected override void OnEffect(List<UIVertex> verts, int pos)
        {
            UIVertex vt;
            float height = 0f;
            if (waveLength > 0)
                height = Mathf.Sin((step * frequency) + (pos / waveLength)) * amplitude;
            for(int i = 0; i < verts.Count; i++)
            {
                vt = verts[i];
                vt.position += Vector3.up * height;
                verts[i] = vt;
            }
        }

        protected override IEnumerator EffectCR()
        {
            while(true)
            {
                step += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}

