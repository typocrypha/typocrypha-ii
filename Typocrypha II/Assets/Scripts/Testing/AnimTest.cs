using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimTest : MonoBehaviour
{
    private void Awake()
    {
        var a = GetComponent<Animator>();
        Debug.Log("Anim test awake detects " + (a != null));
    }
    // Start is called before the first frame update
    void Start()
    {
        var a = GetComponent<Animator>();
        Debug.Log("Anim test start detects " + (a != null));
    }

    // Update is called once per frame
    void Update()
    {
        //var a = GetComponent<Animator>();
        //Debug.Log("Anim test up detects " + (a != null));
    }

    public Animator GetAnim()
    {
        return GetComponent<Animator>();
    }
}
