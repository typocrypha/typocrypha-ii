using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyword : MonoBehaviour
{
    public enum Type
    {
        root, // Root word
        left, // Left modifier
        right, // Right modifier
        bidir // Bidirectional modifier
    }

    public Type type; // Type of this keyword.
    public AudioClip sfx; // SFX played when effect is shown

    void Awake()
    {
        GetComponent<AudioSource>()?.PlayOneShot(sfx);
    }
}
