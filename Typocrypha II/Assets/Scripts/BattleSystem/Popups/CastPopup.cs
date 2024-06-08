using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CastPopup : InteractivePopup
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private AudioClip successSfx;
    [SerializeField] private AudioClip failSfx;
    [SerializeField] CastBarResizer resizer;

    public Spell Spell => lastSpell;
    private Spell lastSpell = null;

    public override void Submit()
    {
        var spellWords = Text.TrimEnd(KeywordDelimiters).Split(KeywordDelimiters);
        var results = SpellParser.instance.Parse(spellWords, SpellCooldownManager.instance.GetSpellsDict(), out lastSpell, out _);
        if(results == SpellParser.ParseResults.Valid)
        {
            var cooldowns = SpellCooldownManager.instance;
            if(cooldowns.IsOnCooldown(lastSpell, out _))
            {
                LastPromptSuccess = false;
            }
            else
            {
                LastPromptSuccess = true;
                cooldowns.DoCooldowns(lastSpell);
            }
        }
        else
        {
            LastPromptSuccess = false;
        }
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
        Prompt = string.Empty;
        gameObject.SetActive(true);
        resizer.Resize(22);
        Resize(22);
        Clear(true);
        InputManager.Instance.StartInput(this);
    }
}
