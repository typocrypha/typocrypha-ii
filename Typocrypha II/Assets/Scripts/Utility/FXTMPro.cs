using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXTMPro : MonoBehaviour
{
    public TMPro.TextMeshPro text;

    private void Start()
    {
        
    }

    private void Update()
    {
        Color32[] colors = new Color32[text.mesh.vertexCount];
        for (int i = 4; i < text.mesh.vertexCount; i++)
        {
            colors[i] = new Color32(255, 0, 0, 255);
        }
        text.mesh.colors32 = colors;
        //text.ForceMeshUpdate();
        //text.UpdateVertexData();
        //text.SetVerticesDirty();
    }
}
