using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemChat : DialogItem
{
    public string speakerName; // Name of speaker (CHAT MODE)
    public Sprite leftIcon; // Icon for left speaker icon (CHAT MODE) 
    public Sprite rightIcon; // Icon for right speaker icon (CHAT MODE)
    public IconSide iconSide; // Side where icon shows (CHAT MODE)
    public DialogItemChat(string text, List<AudioClip> voice, string speakerName, IconSide iconSide, 
                          Sprite leftIcon = null, Sprite rightIcon = null) : base(text, voice)
    {
        this.speakerName = speakerName;
        this.iconSide = iconSide;
        this.leftIcon = leftIcon;
        this.rightIcon = rightIcon;
    }

    public override System.Type GetView()
    {
        return typeof(DialogViewChat);
    }
}
