using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Scrambles letters pseudo-randomly
    public class Scramble : FXTextBase
    {
        public float interval = 0.08f; // Amount of time between changes.
        int[] rand = new int[randLen];

        const int randLen = 32;

        protected override IEnumerator EffectCR()
        {
            while(true)
            {
                for (int i = 0; i < randLen; i++)
                    rand[i] = Random.Range(0, textComp.font.characterInfo.Length);
                yield return new WaitForSeconds(interval);
            }
        }

        protected override void OnEffect(List<UIVertex> verts, int pos)
        {
            UIVertex[] vt = new UIVertex[vertsInQuad];
            CharacterInfo randChar = textComp.font.characterInfo[rand[pos%randLen]];
            for (int i = 0; i < verts.Count; i++)
                vt[i] = verts[i];
            for (int i = 0; i < verts.Count; i+=vertsInQuad)
            {
                vt[i].uv0 = randChar.uvTopLeft;
                vt[i+1].uv0 = randChar.uvTopRight;
                vt[i+2].uv0 = randChar.uvBottomRight;
                vt[i+3].uv0 = randChar.uvBottomRight;
                vt[i+4].uv0 = randChar.uvBottomLeft;
                vt[i+5].uv0 = randChar.uvTopLeft;
            }
            for (int i = 0; i < verts.Count; i++)
                verts[i] = vt[i];
        }
    }
}

