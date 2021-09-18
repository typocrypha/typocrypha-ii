using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemVNPlus : DialogItem
{
    public bool IsLeft { get; }
    public DialogItemVNPlus(string text, List<AudioClip> voice, bool isLeft) : base(text, voice)
    {
        IsLeft = isLeft;
    }
    public override Type GetView()
    {
        return typeof(DialogViewVNPlus);
    }

}
