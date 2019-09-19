using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
        AllyDisplay.instance.gameObject.SetActive(true);
        foreach(var button in spellButtons)
            button.gameObject.SetActive(false);
        bool toggle = true;
        for(int i = 0; i < Spells.Count; ++i)
        {
            bool active = ally.Mp >= Spells[i].Cost;
            spellButtons[i].gameObject.SetActive(true);
            spellButtons[i].interactable = active;
            var spellText = spellButtons[i].GetComponentInChildren<Text>();
            spellText.text = Spells[i].ToDisplayString();
            spellText.color = active ? Color.white : Color.red;
            spellButtons[i].transform.GetChild(1).GetComponent<Text>().text = ((int)Spells[i].Cost).ToString();
            spellButtons[i].transform.GetChild(2).GetComponent<Image>().sprite = Spells[i].Icon;
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
            AllyDisplay.instance.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates display to show currently selected spell's description.
    /// </summary>
    public void UpdateDescription()
    {
        for (int i = 0; i < Spells.Count; ++i)
        {
            if (EventSystem.current.currentSelectedGameObject == spellButtons[i].gameObject)
            {
                AllyDisplay.instance.descriptionText.text = Spells[i][0].description;
                return;
            }
        }
    }

    void Update()
    {
        UpdateDescription();
    }
}
