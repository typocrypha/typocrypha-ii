using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXText
{
    /// <summary>
    /// Text effect for highliting TIPS searchable terms in dialog.
    /// </summary>
    public class TIPS : FXTextBase
    {
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
                vt.color = UnityEngine.Color.blue;
                verts[i] = vt;
            }
        }
    }
}

