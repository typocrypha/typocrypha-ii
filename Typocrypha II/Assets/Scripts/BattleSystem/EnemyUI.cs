using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages enemy UI for an enemy caster.
/// </summary>
public class EnemyUI : MonoBehaviour
{
    public Caster caster;
    public UnityEvent_float onHealthChanged; // Pass percentage health.
    public UnityEvent_string onStaggerChanged; // Pass absolute stagger as string.
    public UnityEvent_string onSpellChanged; // Pass name of spell currently being cast.
}

[System.Serializable]
public class UnityEvent_int : UnityEvent<int> { }
[System.Serializable]
public class UnityEvent_float : UnityEvent<float> { }
[System.Serializable]
public class UnityEvent_string : UnityEvent<string> { }