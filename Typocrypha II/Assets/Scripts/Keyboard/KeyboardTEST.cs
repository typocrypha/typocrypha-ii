using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Testing script.
/// </summary>
public class KeyboardTEST : MonoBehaviour
{
    public Typocrypha.Keyboard keyboard;
    public GameObject effect;
    public string effectString;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        Typocrypha.Keyboard.instance?.ApplyEffect(effectString, effect);
    }
}
