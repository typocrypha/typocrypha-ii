using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Change the color of the selected text
    //[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class TMProColor : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI text; // Text component attached
        public List<int> ind; // List of text indices to apply effect on: in between consecutive pairs
        public Color32 color; // Color of text

        protected const int vertsInQuad = 4; // Number of vertices in a single text quad.
        static readonly Color32 white = new Color32(255, 255, 255, 255);

        private void Update()
        {
            for (int charIndex = 0; charIndex < text.textInfo.characterCount && charIndex < text.text.Length; ++charIndex)
            {
                if (char.IsWhiteSpace(text.text[charIndex]))
                {
                    continue;
                }
                int meshIndex = text.textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = text.textInfo.characterInfo[charIndex].vertexIndex;
                Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
                bool inPair = false;
                for(int pairIndex = 0; pairIndex < ind.Count / 2; pairIndex += 2)
                {
                    if(charIndex >= ind[pairIndex] && charIndex < ind[pairIndex + 1])
                    {
                        inPair = true;
                        SetVertexColors(vertexColors, vertexIndex, color);
                        break;
                    }
                }
                if (!inPair)
                {
                    SetVertexColors(vertexColors, vertexIndex, white);
                }
            }
            text.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.All);
        }

        private void SetVertexColors(Color32[] vertexColors, int vertexIndex, Color32 newColor)
        {
            for (int quadIndex = 0; quadIndex < vertsInQuad; quadIndex++)
            {
                vertexColors[vertexIndex + quadIndex] = newColor;
            }
        }
    }
}

