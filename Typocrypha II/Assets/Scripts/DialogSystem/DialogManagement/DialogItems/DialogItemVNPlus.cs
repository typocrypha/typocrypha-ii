using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemVNPlus : DialogItem
{
    public bool IsLeft { get; }
    public IEnumerable<CharacterData> CharacterData { get; }
    public DialogItemVNPlus(string text, List<AudioClip> voice, bool isLeft, IEnumerable<CharacterData> characterData) : base(text, voice)
    {
        IsLeft = isLeft;
        CharacterData = characterData;
    }
    public override Type GetView()
    {
        return typeof(DialogViewVNPlus);
    }

}
