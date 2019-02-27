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
                vt.color = color;
                verts[i] = vt;
            }
        }
    }
}

