using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellFxWait : SpellFx
{
    public float seconds;
    public override IEnumerator PlayEffect()
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
