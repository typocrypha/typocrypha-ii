using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemVNPlus : DialogItem
{
    public List<CharacterData> CharacterData { get; }
    public DialogItemVNPlus(string text, List<AudioClip> voice, List<CharacterData> characterData) : base(text, voice)
    {
        CharacterData = characterData;
    }
    public override Type GetView()
    {
        return typeof(DialogViewVNPlus);
    }

}
