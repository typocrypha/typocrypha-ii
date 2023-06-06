using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PromptPopup : InteractivePopup
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI promptText;
    public TMP_InputField inputField;

    public override void Focus()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    public override void Unfocus()
    {
        inputField.DeactivateInputField();
    }

    public void OnSubmit()
    {
        LastPromptSuccess = inputField.text.ToUpper() == promptText.text.ToUpper();
        Completed = true;
        InputManager.Instance.CompleteInput();
    }

    protected override void Setup(string header, string prompt, float time)
    {
        Completed = false;
        LastPromptSuccess = false;
        headerText.text = header;
        promptText.text = prompt;
        inputField.text = string.Empty;
        gameObject.SetActive(true);
        InputManager.Instance.StartInput(this);
    }
}
