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
    [SerializeField] private TextMeshProUGUI SpellText; // Text displaying charging spell.
    [SerializeField] private FXText.TMProColor CounterableHighlight; // Highlights for counterable spellwords.
    [SerializeField] private FXText.TMProColor CounteredHighlight; // Highlight for currently countered display.

    void Update()
    {
        // check to see if we are currently countered
        if (SpellText.text == SpellManager.instance.counterWord.internalName.ToUpper())
        {
            CounteredHighlight.ind[0] = 0;
            CounteredHighlight.ind[1] = SpellManager.instance.counterWord.internalName.Length;
            CounteredHighlight.enabled = true;
            CounterableHighlight.enabled = false;
            return;
        }
        else
        {
            CounteredHighlight.enabled = false;
        }
        CounterableHighlight.enabled = true;
        string[] spells = SpellText.text.Split(Spell.separators);
        int pos = 0; // Text character position in spell words.
        // check for any words that can be countered, and highlight them appropriately if found
        for (int i = 0; i < spells.Length; i++)
        {
            string spell = spells[i];
            int index = i * 2;
            if (PlayerDataManager.instance.equipment.EquippedWordsDict.ContainsKey(spell.ToLower()) && !SpellCooldownManager.instance.IsOnCooldown(spell))
            {
                CounterableHighlight.ind[index] = pos;
                CounterableHighlight.ind[index + 1] = pos + spell.Length;
            }
            else
            {
                CounterableHighlight.ind[index] = CounterableHighlight.ind[index + 1] = 0;
            }
            pos += spell.Length + 1;
        }
    }
}
