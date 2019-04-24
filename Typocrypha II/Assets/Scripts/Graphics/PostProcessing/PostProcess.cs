using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic post processing effect on camera.
/// </summary>
public class PostProcess : MonoBehaviour
{
	public static Dictionary<string, PostProcess> postProcessMap 
        = new Dictionary<string, PostProcess>(); // List of all post process effects
    public Material mat; // Material for shader effect
    public Dictionary<string, float> shaderParams = new Dictionary<string, float>(); // Shader property parameters (mapped by name)

	void Awake()
    {
        postProcessMap[mat.name] = this;
	}

	void Update()
    {
        foreach (var kvp in shaderParams)
        {
            mat.SetFloat(kvp.Key, kvp.Value);
        }
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
		Graphics.Blit (src, dest, mat);
	}
}
