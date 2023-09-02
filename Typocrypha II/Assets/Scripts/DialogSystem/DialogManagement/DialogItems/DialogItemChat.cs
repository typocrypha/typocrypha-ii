using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemChat : DialogItemMessage
{
    public IconSide iconSide;
    public string timeText;
    public DialogItemChat(string text, List<AudioClip> voice, List<CharacterData> characterData, string[] characterNames, IconSide iconSide, string timeText) 
        : base(text, voice, characterData, characterNames)
    {
        this.iconSide = iconSide;
        this.timeText = timeText;
    }

    public override System.Type GetView()
    {
        return typeof(DialogViewChat);
    }
}
