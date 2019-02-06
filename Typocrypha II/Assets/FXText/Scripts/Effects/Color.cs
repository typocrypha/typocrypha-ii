using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Change the color of the selected text
    public class Color : FXTextBase
    {
        public Color32 color; // Color of text

        protected override IEnumerator effectCR()
        {
            yield return null;
        }

        protected override void onEffect(VertexHelper vh, int pos)
        {
            int start = pos * 4;
            for (int k = start; k < start + 4; k++)
            {
                UIVertex vert = new UIVertex();
                vh.PopulateUIVertex(ref vert, k);
                vert.color = color;
                vh.SetUIVertex(vert, k);
            }
        }
    }
}

