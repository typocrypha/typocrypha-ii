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
    [SerializeField] private GameObject cooldownUI;
    [SerializeField] private Image[] icons; // Hourglass icons.
    [SerializeField] private Color onColor; // Color when on cooldown.
    [SerializeField] private Color offColor; // Color when off cooldown.
    [SerializeField] private Color bonusColor; // Color when bonus cooldown
    [SerializeField] private GameObject overflowUI;
    [SerializeField] private TMPro.TextMeshProUGUI overflowNumberText;
    [Header("Fixed Use Fields")]
    [SerializeField] private GameObject fixedUseUI;
    [SerializeField] private TMPro.TextMeshProUGUI fixedUseNumberText;

    public int FullCooldown
    {
        get => fullCooldown;
        set
        {
            fullCooldown = value;
            for (int i = 0; i < icons.Length; i++)
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
        private set
        {
            cooldown = System.Math.Max(value, 0);
            // Large Bonus Cooldown!
            if(cooldown > icons.Length)
            {
                foreach(var icon in icons)
                {
                    icon.gameObject.SetActive(false);
                }
                overflowNumberText.text = cooldown.ToString();
                overflowUI.SetActive(true);
            }
            else if(cooldown > FullCooldown)
            {
                // Bonus Cooldown!
                overflowUI.SetActive(false);
                for (int i = 0; i < icons.Length; i++)
                {
                    if(i < FullCooldown)
                    {
                        icons[i].gameObject.SetActive(true);
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
                overflowUI.SetActive(false);
                for (int i = 0; i < FullCooldown; i++)
                {
                    icons[i].color = i < cooldown ? onColor : offColor;
                    icons[i].gameObject.SetActive(true);
                }
                for (int i = FullCooldown; i < icons.Length; i++)
                {
                    icons[i].gameObject.SetActive(false);
                }
            }

            spellText.color = cooldown > 0 ? textOnColor : textOffColor;
        }
    }
    private int cooldown;

    public bool OnCooldown => Cooldown > 0;

    public string SpellText
    {
        get => spellText.text;
        set => spellText.text = value;
    }

    public bool IsFixedUse { get; private set; }

    public int Uses 
    {
        get => uses;
        set
        {
            uses = System.Math.Min(value, MaxUses);
            fixedUseNumberText.text = uses.ToString();
        } 
    }
    private int uses;

    public int MaxUses { get; private set; }

    public bool OnCast(int cooldown)
    {
        if (IsFixedUse)
        {
            Uses -= 1;
            return Uses <= 0;
        }
        Cooldown += cooldown;
        return false;
    }

    public void ResetCooldown()
    {
        Cooldown = 0;
    }

    public void LowerCooldown(int amount)
    {
        Cooldown -= amount;
    }

    public void SetupCooldown(int fullCooldown)
    {
        FullCooldown = fullCooldown;
        Cooldown = 0;
        cooldownUI.SetActive(true);
        fixedUseUI.SetActive(false);
        IsFixedUse = false;
    }

    public void SetupWithFixedUses(int uses, int maxUses)
    {
        MaxUses = maxUses;
        Uses = uses;
        cooldownUI.SetActive(false);
        fixedUseUI.SetActive(true);
        IsFixedUse = true;
    }

    public SpellWord SpellWord { get; set; }
}
