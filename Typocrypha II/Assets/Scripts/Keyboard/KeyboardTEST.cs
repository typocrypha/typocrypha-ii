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

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        keyboard.ApplyEffect("qwe", effect);
        yield return new WaitForSeconds(2f);
        keyboard.ApplyEffect("ahv", effect);
    }
}
