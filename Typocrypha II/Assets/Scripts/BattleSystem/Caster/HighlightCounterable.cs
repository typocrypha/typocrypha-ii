using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Highlight counterable spells in enemy spell cast text.
/// </summary>
[RequireComponent(typeof(Text))]
[RequireComponent(typeof(FXText.Color))]
public class HighlightCounterable : MonoBehaviour
{
    public Text SpellText; // Text displaying charging spell.
    public FXText.Color[] Highlights; // Highlights for spellwords.
    public FXText.Color CounteredHighlight; // Highlight for countered display.

    void Update()
    {
        string[] spells = SpellText.text.Split('-');
        int pos = 0; // Text character position in spell words.
        for (int i = 0; i < spells.Length; i++)
        {
            string spell = spells[i];
            try
            {
                if (!SpellCooldownManager.instance.IsOnCooldown(spell))
                {
                    Highlights[i].ind[0] = pos;
                    Highlights[i].ind[1] = pos + spell.Length;
                }
                else
                {
                    Highlights[i].ind[0] = 0;
                    Highlights[i].ind[1] = 0;
                }
            }
            catch (UnityException e)
            {
                Highlights[i].ind[0] = 0;
                Highlights[i].ind[1] = 0;
            }
            pos += spell.Length + 1;
        }

        if (SpellText.text == SpellManager.instance.counterWord.internalName.ToUpper())
        {
            CounteredHighlight.ind[0] = 0;
            CounteredHighlight.ind[1] = SpellManager.instance.counterWord.internalName.Length;
        }
        else
        {
            CounteredHighlight.ind[0] = 0;
            CounteredHighlight.ind[1] = 0;
        }
    }
}
