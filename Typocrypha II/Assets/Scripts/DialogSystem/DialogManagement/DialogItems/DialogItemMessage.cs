using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogItemMessage : DialogItem
{
    public List<CharacterData> CharacterData { get; }
    public DialogItemMessage(string text, List<AudioClip> voice, List<CharacterData> characterData, string[] characterNames) : base(text, voice)
    {
        CharacterData = characterData;
        for (int i = 0; i < CharacterData.Count && i < characterNames.Length; i++)
        {
            if(CharacterData[i] == null)
            {
                this.text = $"{characterNames[i]}: {this.text}";
                this.voice = null;
            }
        }
    }
}
