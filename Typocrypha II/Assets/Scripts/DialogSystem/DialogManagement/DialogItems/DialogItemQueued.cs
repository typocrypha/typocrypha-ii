using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemQueued : DialogItem
{
    public DialogItemQueued() : base(string.Empty, null)
    {

    }
    public override Type GetView()
    {
        return null;
    }
}
