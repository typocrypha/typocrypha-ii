using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectAddSpellWord : BadgeEffect
{
    [SerializeField] private SpellWord word;

    public override void Equip(Player player)
    {
        PlayerDataManager.Equipment.EquipWord(word);
        SpellCooldownManager.instance.AddWord(word, true);
    }

    public override void Unequip(Player player)
    {
        PlayerDataManager.Equipment.UnequipWord(word);
        SpellCooldownManager.instance.RemoveWord(word);
    }
}
