using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

namespace FXText
{
    /// <summary>Change the color of the selected text</summary> 
    public class TMProColor : TMProEffect
    {
        public Color32 color; // Color of text
        static readonly Color32 defaultColor = new Color32(255, 255, 255, 255); // Default color

        public override PriorityGroupEnum PriorityGroup => PriorityGroupEnum.COLOR;

        protected override void ApplyVertexEffect(TMP_MeshInfo meshInfo, int vertexIndex)
        {
            Color32[] vertexColors = meshInfo.colors32;
            for (int quadIndex = 0; quadIndex < vertsInQuad; quadIndex++)
            {
                vertexColors[vertexIndex + quadIndex] = color;
            }
        }

        protected override void ApplyDefaultEffect(TMP_MeshInfo meshInfo, int vertexIndex)
        {
            Color32[] vertexColors = meshInfo.colors32;
            for (int quadIndex = 0; quadIndex < vertsInQuad; quadIndex++)
            {
                vertexColors[vertexIndex + quadIndex] = defaultColor;
            }
        }
    }
}

