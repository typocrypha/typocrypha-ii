using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Calls a UnityEvent handle when script is enabled.
/// </summary>
public class OnEnabled : MonoBehaviour
{
    public UnityEvent onEnabledEvent;
    public bool late = false; // Should event be called in late update?

    bool lateSwitch = false;

    void OnEnable()
    {
        if (!late) onEnabledEvent.Invoke();
        else lateSwitch = true;
    }

    void LateUpdate()
    {
        if (lateSwitch)
        {
            lateSwitch = false;
            onEnabledEvent.Invoke();
        }
    }
}
