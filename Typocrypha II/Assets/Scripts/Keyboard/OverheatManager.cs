using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverheatManager : MonoBehaviour, IPausable
{
    #region IPausable
    public PauseHandle PH { get; private set; }
    // Stops input and pauses keyboard effects.
    public void OnPause(bool b)
    {
        if (!IsOverheating)
            return;
        if (b)
        {
            inputField.interactable = false;
        }
        else
        {
            inputField.interactable = true;
            inputField.Select();
        }
    }
    #endregion

    public bool IsOverheating => ui.enabled;

    [SerializeField] Typocrypha.Keyboard keyboard;
    [SerializeField] private WordList overheatWords;
    [Header("UI References")]
    [SerializeField] private Canvas ui;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TMP_InputField inputField;
    [Header("SFX")]
    [SerializeField] private AudioClip successSFX;
    [SerializeField] private AudioClip failSFX;

    private WordListSelector wordSelector;

    private void Awake()
    {
        wordSelector = new WordListSelector(overheatWords.Words);
        PH = new PauseHandle(OnPause);
        PH.SetParent(keyboard);
    }

    public void DoOverheat()
    {
        if (IsOverheating)
            return;
        ui.enabled = true;
        inputField.interactable = true;
        DoOverheatInternal();
    }

    public void StopOverheat()
    {
        if (!IsOverheating)
            return;
        inputField.interactable = false;
        inputField.DeactivateInputField(true);
        ui.enabled = false;
    }

    public void OnSubmit(string input)
    {
        if(input.ToUpper() != promptText.text)
        {
            // Failure
            AudioManager.instance.PlaySFX(failSFX);
            inputField.text = string.Empty;
            inputField.Select();
            inputField.ActivateInputField();
            return;
        }
        // Success!
        AudioManager.instance.PlaySFX(successSFX);
        // Do cooldown
        SpellCooldownManager.instance.LowerAllCooldowns(1);
        SpellCooldownManager.instance.SortCooldowns();
        if (SpellCooldownManager.instance.Overheated)
        {
            // Do another overheat if still overheated
            DoOverheatInternal();
        }
        else
        {
            StopOverheat();
        }
    }

    private void DoOverheatInternal()
    {
        promptText.text = wordSelector.Get().ToUpper();
        inputField.text = string.Empty;
        inputField.Select();
        inputField.ActivateInputField();
    }
}
