using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverheatManager : Typocrypha.CastBar
{
    public bool IsOverheating => ui.enabled;

    [SerializeField] Typocrypha.Keyboard keyboard;
    [SerializeField] private WordList overheatWords;
    [Header("UI References")]
    [SerializeField] private Canvas ui;
    [SerializeField] private TextMeshProUGUI promptText;
    [Header("SFX")]
    [SerializeField] private AudioClip successSFX;
    [SerializeField] private AudioClip failSFX;

    private WordListSelector wordSelector;

    protected override void Awake()
    {
        base.Awake();
        wordSelector = new WordListSelector(overheatWords.Words);
        PH.SetParent(keyboard);
    }

    public void DoOverheat()
    {
        if (IsOverheating)
            return;
        ui.enabled = true;
        DoOverheatInternal();
        InputManager.Instance.StartInput(this);
    }

    public void StopOverheat()
    {
        if (!IsOverheating)
            return;
        ui.enabled = false;
        InputManager.Instance.CompleteInput();
    }

    public override void Submit()
    {
        if (sb.ToString().ToUpper() != promptText.text)
        {
            // Failure
            AudioManager.instance.PlaySFX(failSFX);
            Clear(true);
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
        Clear(true);
    }
}
