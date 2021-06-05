using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptPopup : InteractivePopup
{
    public Text headerText;
    public Text promptText;
    public InputField inputField;

    public void OnSubmit()
    {
        LastPromptSuccess = inputField.text.ToUpper() == promptText.text.ToUpper();
        Completed = true;
    }

    protected override void Setup(string header, string prompt, float time)
    {
        Completed = false;
        LastPromptSuccess = false;
        headerText.text = header;
        promptText.text = prompt;
        inputField.text = string.Empty;
        gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
    }
}
