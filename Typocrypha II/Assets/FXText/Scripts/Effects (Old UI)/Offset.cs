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

        protected override IEnumerator EffectCR()
        {
            yield return null;
        }

        protected override void OnEffect(List<UIVertex> verts, int pos)
        {
            UIVertex vt;
            for (int i = 0; i < verts.Count; i++)
            {
                vt = verts[i];
                vt.position += offset;
                verts[i] = vt;
            }
        }
    }
}

