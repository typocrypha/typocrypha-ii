using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Manages ally menu for ally casting.
/// </summary>
public class AllyMenu : MonoBehaviour
{
    public ATB3.ATBAlly ally; // Ally this menu is for.

    public List<Spell> Spells;
    private Button[] spellButtons;

    private void Awake()
    {
        spellButtons = GetComponentsInChildren<Button>();
    }

    public bool CanCast => Spells.Any((s) => s.Count != 0 && ally.Mp >= s.Cost);

    public void Activate(ATB3.ATBStateID state)
    {
        bool toggle = true;
        for(int i = 0; i < spellButtons.Length; ++i)
        {
            bool active = Spells.Count > i && ally.Mp >= Spells[i].Cost;
            spellButtons[i].gameObject.SetActive(active);
            spellButtons[i].GetComponentInChildren<Text>().text = Spells[i].ToDisplayString();
            // Select first available spell.
            if (active && toggle)
            {
                toggle = false;
                spellButtons[i].Select();
            }
        }
    }

    public void Cast(int index)
    {
        Spell cast = Spells[index];
        if (cast != null && cast.Count > 0 && ally.Mp >= cast.Cost)
        {
            ally.Cast(cast);
            gameObject.SetActive(false);
        }
    }

}
