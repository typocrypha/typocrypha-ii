using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Moves text (in unison)
    public class Offset : FXTextBase
    {
        public Vector3 offset; // Offset from base position

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
                vert.position += offset;
                vh.SetUIVertex(vert, k);
            }
        }
    }
}

