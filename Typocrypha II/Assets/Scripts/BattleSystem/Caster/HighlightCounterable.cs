using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Highlight counterable spells in enemy spell cast text.
/// </summary>
public class HighlightCounterable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI spellText; // Text displaying charging spell.
    [SerializeField] private TextMeshProUGUI counteredText; // Text that shows when countered
    [SerializeField] private FXText.TMProColor CounterableHighlight; // Highlights for counterable spellwords.

    private bool countered = false;
    private string[] spellWords = Array.Empty<string>();

    public void UpdateCounteredState(bool state)
    {
        if (countered == state)
            return;
        countered = state;
        spellText.gameObject.SetActive(!countered);
        counteredText.gameObject.SetActive(countered);
    }

    public void UpdateSpellWords(string spell)
    {
        spellWords = spell == "" ? Array.Empty<string>() : spell.Split(Spell.separators);
        // Reset counterable indices when spell changes
        for (int i = 0; i < CounterableHighlight.ind.Count; ++i)
        {
            CounterableHighlight.ind[i] = 0;
        }
    }

    void Update()
    {
        // Don't update if countered
        if (countered)
            return;
        int pos = 0; // Text character position in spell words.
        // check for any words that can be countered, and highlight them appropriately if found
        for (int i = 0; i < spellWords.Length; i++)
        {
            string spellWord = spellWords[i];
            int index = i * 2;
            var wordData = Lookup.instance.GetSpellWord(spellWord.ToLower());
            if(wordData == null)
            {
                continue;
            }
            if (PlayerDataManager.instance.CanCastSpell(wordData) && !SpellCooldownManager.instance.IsOnCooldown(wordData))
            {
                CounterableHighlight.ind[index] = pos;
                CounterableHighlight.ind[index + 1] = pos + spellWord.Length;
            }
            else
            {
                CounterableHighlight.ind[index] = CounterableHighlight.ind[index + 1] = 0;
            }
            pos += spellWord.Length + 1;
        }
    }
}
