using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Typocrypha;

/// <summary>
/// Testing script.
/// </summary>
public class KeyboardTEST : MonoBehaviour
{
    public Keyboard keyboard;
    public GameObject effect;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        keyboard.ApplyEffect("qwe", effect);
        yield return new WaitForSeconds(2f);
        keyboard.ApplyEffect("ahv", effect);
    }
}
