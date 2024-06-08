using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PromptPopup : InteractivePopup
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private AudioClip successSfx;
    [SerializeField] private AudioClip failSfx;
    [SerializeField] CastBarResizer resizer;

    public override void Submit()
    {
        LastPromptSuccess = Text.ToUpper() == Prompt.ToUpper();
        AudioManager.instance.PlaySFX(LastPromptSuccess ? successSfx : failSfx);
        Completed = true;
        Battlefield.instance.Player.OnPromptComplete?.Invoke(LastPromptSuccess);
        InputManager.Instance.CompleteInput();
    }

    protected override void OnTimeout()
    {
        base.OnTimeout();
        AudioManager.instance.PlaySFX(failSfx);
        Battlefield.instance.Player.OnPromptComplete?.Invoke(false);
    }

    protected override void Setup(string header, string prompt, float time)
    {
        Completed = false;
        LastPromptSuccess = false;
        headerText.text = header;
        Prompt = prompt;
        gameObject.SetActive(true);
        resizer.Resize(Prompt.Length);
        Resize(Prompt.Length);
        Clear(true);
        InputManager.Instance.StartInput(this);
    }
}
