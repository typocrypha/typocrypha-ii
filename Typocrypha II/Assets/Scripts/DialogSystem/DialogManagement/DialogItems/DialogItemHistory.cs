using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemHistory : DialogItem
{
    public DialogItemHistory(string text, List<AudioClip> voice) : base(text, voice)
    {
    }

    public override Type GetView()
    {
        return typeof(DialogHistory);
    }
}
