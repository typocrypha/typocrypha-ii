using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of a single spell cooldown. Mostly controls UI.
/// </summary>
public class SpellCooldown : MonoBehaviour
{
    [Header("Spell Text Fields")]
    [SerializeField] private TMPro.TextMeshProUGUI spellText; // Text for spell name.
    [SerializeField] private Color textOnColor;
    [SerializeField] private Color textOffColor;
    [Header("Icon Fields")]
    [SerializeField] private Image[] icons; // Hourglass icons.
    [SerializeField] private Color onColor; // Color when on cooldown.
    [SerializeField] private Color offColor; // Color when off cooldown.
    [SerializeField] private Color bonusColor; // Color when bonus cooldown

    public int MaxCooldown => icons.Length;
    public int FullCooldown
    {
        get => fullCooldown;
        set
        {
            fullCooldown = value;
            for (int i = 0; i < MaxCooldown; i++)
            {
                icons[i].gameObject.SetActive(i < fullCooldown);
                icons[i].color = offColor;
            }
        }
    }
    int fullCooldown;

    public int Cooldown
    {
        get => cooldown;
        set
        {
            cooldown = Mathf.Clamp(value, 0, MaxCooldown);
            // Bonus Cooldown!
            if(cooldown > FullCooldown)
            {
                for (int i = 0; i < MaxCooldown; i++)
                {
                    if(i < FullCooldown)
                    {
                        icons[i].color = onColor;
                    }
                    else if(i < cooldown)
                    {
                        icons[i].gameObject.SetActive(true);
                        icons[i].color = bonusColor;
                    }
                    else
                    {
                        icons[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // Normal Cooldown
                for (int i = 0; i < FullCooldown; i++)
                {
                    icons[i].color = i < cooldown ? onColor : offColor;
                }
                for (int i = FullCooldown; i < MaxCooldown; i++)
                {
                    icons[i].gameObject.SetActive(false);
                }
            }

            spellText.color = cooldown > 0 ? textOnColor : textOffColor;
        }
    }
    private int cooldown;

    public string SpellText
    {
        get => spellText.text;
        set => spellText.text = value;
    }
}
