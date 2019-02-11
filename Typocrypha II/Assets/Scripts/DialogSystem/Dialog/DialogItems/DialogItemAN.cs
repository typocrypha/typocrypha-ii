using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemAN : DialogItem
{
    public DialogItemAN(string text) : base(text) { }

    public override System.Type GetView()
    {
        return typeof(DialogViewAN);
    }
}
