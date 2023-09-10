using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatCharacter : MonoBehaviour, IComparable<ChatCharacter>
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private TextMeshProUGUI usernameText;
    public CharacterData Data
    {
        get => characterData;
        set
        {
            characterData = value;
            icon.sprite = characterData.chat_icon;
            displayNameText.text = characterData.chatDisplayName;
            usernameText.text = characterData.chatUsername;
        }
    }
    private CharacterData characterData;

    public int CompareTo(ChatCharacter other)
    {
        if (gameObject.activeInHierarchy)
        {
            if (other.gameObject.activeInHierarchy)
            {
                return displayNameText.text.CompareTo(other.displayNameText.text);
            }
            return -1;
        }
        else if(other.gameObject.activeInHierarchy)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
