using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic post processing effect on camera.
/// </summary>
public class PostProcess : MonoBehaviour
{
	public static Dictionary<string, PostProcess> postProcessMap = null; // List of all post process effects
    public Material mat; // Material for shader effect
    public Dictionary<string, float> shaderParams; // Shader property parameters (mapped by name)

	void Awake()
    {
        if (postProcessMap == null)
            postProcessMap = new Dictionary<string, PostProcess>();
        if (!postProcessMap.ContainsKey(mat.name)) postProcessMap[mat.name] = this;
        else return;
        shaderParams = new Dictionary<string, float>();
	}

	void Update()
    {
        foreach (var kvp in shaderParams)
        {
            mat.SetFloat(kvp.Key, kvp.Value);
        }
	}

    void OnDestroy()
    {
        Debug.Log("Destroy post process");
    }

	void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
		Graphics.Blit (src, dest, mat);
	}
}
