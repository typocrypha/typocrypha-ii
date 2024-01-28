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
    public UnityEvent_string onHealthChangedNumber; // Pass absolute health as string.
    public UnityEvent_float onSpChanged; // Pass percentage SP
    public UnityEvent_float onChargeChanged; // Pass percentage charge.
    public UnityEvent_string onChargeChangedNumber; // Pass absolute charge / MP as string.
    public UnityEvent_string onStaggerChanged; // Pass absolute stagger as string.
    public UnityEvent_string onNameChanged; // Pass name as string
    public UnityEvent onStun; // Call when stunned.
    public UnityEvent onUnstun;
    public UnityEvent_float onStunProgressChanged;
    public UnityEvent onSpiritForm; // Call when entering spirit form
    public UnityEvent_string onSpellChanged; // Pass name of spell currently being cast.
    public UnityEvent_sprite onSpellIconChanged; // Pass icon of spell current being cast.
    public UnityEvent_sprite onSpriteChanged; // Pass new sprite
    public UnityEvent_bool onCounterStateChanged;
    public UnityEvent_string onScouterDataChanged;
    public UnityEvent onScouterShow;
    public UnityEvent onScouterHide;
    public UnityEvent onDimEnabled;
    public UnityEvent onDimDisabled;
}
