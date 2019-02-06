using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Scrambles letters pseudo-randomly
    public class Scramble : FXTextBase
    {
        public float interval; // Amount of time between changes
        int seed;

        protected override IEnumerator effectCR()
        {
            while(true)
            {
                seed += 7;
                yield return new WaitForSeconds(interval);
            }
        }

        protected override void onEffect(VertexHelper vh, int pos)
        {
            CharacterInfo randChar = textComp.font.characterInfo[((seed + pos * 11) % 199) % textComp.font.characterInfo.Length];
            int start = pos * 4;

            UIVertex[] vert = new UIVertex[4];
            for (int i = 0; i < 4; i++) vh.PopulateUIVertex(ref vert[i], start + i);
            vert[0].uv0 = randChar.uvTopLeft;
            vert[1].uv0 = randChar.uvTopRight;
            vert[2].uv0 = randChar.uvBottomRight;
            vert[3].uv0 = randChar.uvBottomLeft;
            for (int i = 0; i < 4; i++) vh.SetUIVertex(vert[i], start + i);
        }
    }
}

