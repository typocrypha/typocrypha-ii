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
            TMPro.TMP_WordInfo info = text.textInfo.wordInfo[0];
            for (int i = 0; i < text.textInfo.characterCount; ++i)
            {
                int charIndex = i;
                if (char.IsWhiteSpace(text.text[charIndex]))
                {
                    continue;
                }
                int meshIndex = text.textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = text.textInfo.characterInfo[charIndex].vertexIndex;
                Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
                for (int j = 0; j < vertsInQuad; j++)
                {
                    if (i >= ind[0] && i < ind[1])
                        vertexColors[vertexIndex + j] = color;
                    else
                        vertexColors[vertexIndex + j] = white;
                }
            }
            text.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.All);
        }
    }
}

