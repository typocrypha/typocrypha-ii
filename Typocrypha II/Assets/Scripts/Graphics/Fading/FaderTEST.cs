using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaderTEST : MonoBehaviour
{
    public Fader fader;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        FaderManager.instance.Solo(fader, 0.5f, Color.black);
    }
}
