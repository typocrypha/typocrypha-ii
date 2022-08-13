using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    /// <summary>
    /// Rainbow xolor changing effect.
    /// </summary>
    public class Rainbow : FXTextBase
    {
        public float period = 0.1f; // Time period between changes.

        int step;
        static UnityEngine.Color[] rainbowColors;

        protected override IEnumerator EffectCR()
        {
            if (rainbowColors == null)
            {
                rainbowColors = new UnityEngine.Color[]
                {
                    UnityEngine.Color.red,
                    new UnityEngine.Color(1f,0.5f,0f,1f),
                    UnityEngine.Color.yellow,
                    UnityEngine.Color.green,
                    UnityEngine.Color.blue,
                    new UnityEngine.Color(1f,0f,1f,1f)
                };
            }

            while(true)
            {
                step++;
                yield return new WaitForSeconds(period);
            }
        }

        protected override void OnEffect(List<UIVertex> verts, int pos)
        {
            UIVertex vt;
            for (int i = 0; i < verts.Count; i++)
            {
                vt = verts[i];
                vt.color = rainbowColors[(pos + step) % rainbowColors.Length];
                verts[i] = vt;
            }
        }
    }
}

