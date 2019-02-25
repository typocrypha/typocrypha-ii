using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages behavior of spacebar icon during dialog.
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class DialogSpacebar : MonoBehaviour
{
    SpriteRenderer sr;
    Animator animator;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Check state of dialog, and update visuals appropriately.
    void Update()
    {

    }
}
