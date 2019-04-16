using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellFxBase : MonoBehaviour
{
    public abstract IEnumerator PlayEffect();
}
