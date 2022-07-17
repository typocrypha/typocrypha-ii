using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FXText
{
    /// <summary>Change the color of the selected text</summary> 
    /// <remark>
    /// Hacked it to fit within FXTextBase base class, however overrides most functionality
    /// </remark>
    public class TMProColor : MonoBehaviour
    {
        /// <summary>
        /// Enum for when effect runs
        /// </summary>
        public enum PriorityEnum
        {
            Update,
            LateUpdate,
        }

        public TMPro.TextMeshProUGUI text; // Text component attached
        public List<int> ind; // List of text indices to apply effect on: in between consecutive pairs
        public Color32 color; // Color of text
        public int Priority = 0; // Priority of color override when multiple are working. Higher priority = displayed.

        const int vertsInQuad = 4; // Number of vertices in a single text quad.
        static readonly Color32 white = new Color32(255, 255, 255, 255);

        private void Update()
        {
            // Get all other color components to determine priority
            var allColors = GetComponents<TMProColor>();
            allColors.OrderBy(a => a.Priority);
            // All effects managed by highest priority instance
            if (allColors.Last() == this)
            {
                UpdateMesh(text, allColors);
            }
        }

        static void UpdateMesh(TMPro.TextMeshProUGUI text, IEnumerable<TMProColor> allColors)
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
                // Set color (by priority) for highest priority
                for (int priority = 0; priority < allColors.Count(); priority++)
                {
                    var instance = allColors.ElementAt(priority);
                    for (int pairIndex = 0; pairIndex < instance.ind.Count / 2; pairIndex += 2)
                    {
                        if (charIndex >= instance.ind[pairIndex] && charIndex < instance.ind[pairIndex + 1])
                        {
                            inPair = true;
                            SetVertexColors(vertexColors, vertexIndex, instance.color);
                            break;
                        }
                    }
                    if (inPair) break;
                }
                // Default color (when no color applies)
                if (!inPair)
                {
                    SetVertexColors(vertexColors, vertexIndex, white);
                }
            }
            text.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.All);
        }

        static void SetVertexColors(Color32[] vertexColors, int vertexIndex, Color32 newColor)
        {
            for (int quadIndex = 0; quadIndex < vertsInQuad; quadIndex++)
            {
                vertexColors[vertexIndex + quadIndex] = newColor;
            }
        }
    }
}

