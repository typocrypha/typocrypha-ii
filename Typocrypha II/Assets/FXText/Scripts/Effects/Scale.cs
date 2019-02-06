using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Scales individual letters around their averaged center
    public class Scale : FXTextBase
    {
        public Vector3 scale = Vector3.one; // Scale ratio in xyz directions

        protected override IEnumerator effectCR()
        {
            yield return null;
        }

        protected override void onEffect(VertexHelper vh, int pos)
        {
            int start = pos * 4;
            UIVertex[] vert = new UIVertex[4];
            Vector3 center = new Vector3();
            for (int i = 0; i < 4; i++)
            {
                vh.PopulateUIVertex(ref vert[i], start + i);
                center += vert[i].position;
            }
            center /= 4f;
            for (int i = 0; i < 4; i++)
            {
                vert[i].position -= center;
                vert[i].position = Vector3.Scale(vert[i].position, scale);
                vert[i].position += center;
                vh.SetUIVertex(vert[i], start + i);
            }
        }
    }
}


