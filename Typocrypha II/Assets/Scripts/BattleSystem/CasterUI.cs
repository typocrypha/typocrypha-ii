using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages UI for a caster.
/// </summary>
public class CasterUI : MonoBehaviour
{
    public UnityEvent_float onHealthChanged; // Pass percentage health.
    public UnityEvent_float onChargeChanged; // Pass percentage charge.
    public UnityEvent_string onStaggerChanged; // Pass absolute stagger as string.
    public UnityEvent onStun; // Call when stunned.
    public UnityEvent_string onSpellChanged; // Pass name of spell currently being cast.
}
