using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FXText
{
    /// <summary>
    /// Shake text randomly.
    /// Note: doesnt update dynamically if text itself is updated.
    /// </summary>
    public class TMProShake : TMProEffect
    {
        public float Intensity = 1f; // Amount of shake

        public override PriorityGroupEnum PriorityGroup => PriorityGroupEnum.POSITION;

        Vector3[] basePos; // Base positions

        protected override void ApplyDefaultEffect(TMP_MeshInfo meshInfo, int vertexIndex)
        {
            // Memoize previous positions for resetting position
            if (basePos == null)
            {
                basePos = new Vector3[meshInfo.vertexCount];
                System.Array.Copy(meshInfo.vertices, basePos, meshInfo.vertexCount);
            }
            for (int quadIndex = 0; quadIndex < vertsInQuad; quadIndex++)
            {
                meshInfo.vertices[vertexIndex + quadIndex] = basePos[vertexIndex + quadIndex];
            }
        }

        protected override void ApplyVertexEffect(TMP_MeshInfo meshInfo, int vertexIndex)
        {
            var shift = Random.insideUnitCircle * Intensity;
            for (int quadIndex = 0; quadIndex < vertsInQuad; quadIndex++)
            {
                meshInfo.vertices[vertexIndex + quadIndex] = basePos[vertexIndex + quadIndex] + (Vector3)shift;
            }
        }
    }
}

