using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTriggerEnableKeyboardPrompts : BattleTrigger
{
    [SerializeField] private bool enable;
    [SerializeField] private string promptOverride;
    protected override void Trigger()
    {
        Typocrypha.Keyboard.instance.EnablePrompting(enable, promptOverride);
    }
}
