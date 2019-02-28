using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to 'Selectable' object. Selects object when enabled.
/// </summary>
[RequireComponent(typeof(Selectable))]
public class SelectOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Selectable>().Select();
    }
}
