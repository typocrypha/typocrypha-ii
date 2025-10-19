using System;
using System.IO;
using UnityEngine;

/// <summary>
/// TIPS entry spell word variant
/// </summary>
[CreateAssetMenu]
[System.Serializable]
public class TIPSEntrySpell : TIPSEntryData
{
    public SpellWord spell;
    public override string Title => ID + $"<sprite name={spell.icon.name}>";

    public override void OnValidate()
    {
        base.OnValidate();

        Content = spell.description;

        string style = "<color=#AAAAAA><indent=16><size=14>";
        Content = string.Join("\n", spell.description, style , spell.flavorText);

        string power = $"Power: TODO";
        string timer = "<sprite name=timer_icon_v2>";
        string cooldown = "Cooldown: " + string.Concat(System.Linq.Enumerable.Repeat(timer, spell.cooldown));
        string root = $"Root Word: {spell.BaseName}";
        Footer = string.Join("\n", power, cooldown, root);
    }
}