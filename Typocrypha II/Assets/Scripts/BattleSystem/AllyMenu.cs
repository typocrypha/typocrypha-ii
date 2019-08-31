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

    public Spell spellUp;
    public Spell spellLeft;
    public Spell spellRight;
    public Spell spellDown;
    public List<Spell> Spells => new List<Spell>() { spellUp, spellLeft, spellRight, spellDown };
    private Text[] spellText;

    public bool CanCast => Spells.Any((s) => s.Cost <= ally.Mp);

    public void Activate(ATB3.ATBStateID state)
    {
        spellText = GetComponentsInChildren<Text>();
        var spellObjects = Spells;
        for(int i = 0; i < spellText.Length; ++i)
        {
            spellText[i].gameObject.SetActive(spellObjects.Count > 0);
            spellText[i].text = spellObjects[i].ToDisplayString();
        }
    }

    void Update()
    {
        Spell cast = null;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cast = spellUp;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cast = spellRight;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cast = spellLeft;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cast = spellDown;
        }
        if(cast != null && cast.Count > 0 && ally.Mp >= cast.Cost)
        {
            ally.Cast(cast);
            gameObject.SetActive(false);
        }
    }
}
