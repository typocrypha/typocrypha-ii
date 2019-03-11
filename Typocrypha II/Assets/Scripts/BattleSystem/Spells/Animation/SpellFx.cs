using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellFx : MonoBehaviour
{
    public abstract IEnumerator PlayEffect();
}
