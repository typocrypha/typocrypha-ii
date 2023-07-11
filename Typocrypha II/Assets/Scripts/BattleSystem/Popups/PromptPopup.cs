using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PromptPopup : InteractivePopup
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI promptText;

    public override void Submit()
    {
        LastPromptSuccess = sb.ToString().ToUpper() == promptText.text.ToUpper();
        Completed = true;
        InputManager.Instance.CompleteInput();
    }

    protected override void Setup(string header, string prompt, float time)
    {
        Completed = false;
        LastPromptSuccess = false;
        headerText.text = header;
        promptText.text = prompt;
        gameObject.SetActive(true);
        Clear(true);
        InputManager.Instance.StartInput(this);
    }
}
