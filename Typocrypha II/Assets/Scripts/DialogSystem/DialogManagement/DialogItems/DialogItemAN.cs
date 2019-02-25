using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemAN : DialogItem
{
    public DialogItemAN(string text, AudioClip voice) : base(text, voice) { }

    public override System.Type GetView()
    {
        return typeof(DialogViewAN);
    }
}
