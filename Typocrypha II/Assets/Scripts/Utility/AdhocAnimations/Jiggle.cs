using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jiggles attached object in place.
/// </summary>
public class Jiggle : MonoBehaviour
{
    public float intensity = 0.2f;
    public float delay = 0.02f;
    Vector3 basePos;

    void Start()
    {
        basePos = transform.position;
        StartCoroutine(JiggleCR());
    }
    
    IEnumerator JiggleCR()
    {
        for (; ; )
        {
            transform.position = basePos + Random.insideUnitSphere * intensity;
            yield return new WaitForSeconds(delay);
        }
    }
}
