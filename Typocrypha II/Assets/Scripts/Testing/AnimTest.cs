using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = GetComponent<Animator>();
        Debug.Log("Anim test detects " + (a != null));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
