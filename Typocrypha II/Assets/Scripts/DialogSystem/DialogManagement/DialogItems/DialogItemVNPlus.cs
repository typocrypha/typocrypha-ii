using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemVNPlus : DialogItemMessage
{
    public DialogItemVNPlus(string text, List<AudioClip> voice, List<CharacterData> characterData, string[] characterNames)
        : base(text, voice, characterData, characterNames) { }
    public override Type GetView()
    {
        return typeof(DialogViewVNPlus);
    }
}
