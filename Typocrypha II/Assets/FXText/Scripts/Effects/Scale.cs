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

        protected override IEnumerator EffectCR()
        {
            yield return null;
        }

        protected override void OnEffect(List<UIVertex> verts, int pos)
        {
            UIVertex[] vt = new UIVertex[vertsInQuad];
            Vector3 center = new Vector3();
            for (int i = 0; i < verts.Count/vertsInQuad; i++)
            {
                center = Vector3.zero;
                for (int j = 0; j < vertsInQuad; j++)
                {
                    int p = i * j;
                    vt[p] = verts[p];
                    center += vt[p].position;
                }
                center /= vertsInQuad;
                for (int j = 0; i < vertsInQuad; j++)
                {
                    int p = i * j;
                    vt[p].position -= center;
                    vt[p].position = Vector3.Scale(vt[p].position, scale);
                    vt[p].position += center;
                    verts[p] = vt[p];
                }
            }
        }
    }
}


