using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    public class Rainbow : FXTextBase
    {
        public float period = 0.1f; // Time period between changes.

        int step;
        static UnityEngine.Color[] rainbowColors;
        

        protected override IEnumerator effectCR()
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

        protected override void onEffect(VertexHelper vh, int pos)
        {
            int start = pos * 4;
            for (int k = start; k < start + 4; k++)
            {
                UIVertex vert = new UIVertex();
                vh.PopulateUIVertex(ref vert, k);
                vert.color = rainbowColors[((k/4) + step) % rainbowColors.Length];
                vh.SetUIVertex(vert, k);
            }
        }
    }
}

