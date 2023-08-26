using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemChat : DialogItemMessage
{
    public IconSide iconSide;
    public DialogItemChat(string text, List<AudioClip> voice, List<CharacterData> characterData, string[] characterNames, IconSide iconSide) 
        : base(text, voice, characterData, characterNames)
    {
        this.iconSide = iconSide;
    }

    public override System.Type GetView()
    {
        return typeof(DialogViewChat);
    }
}
