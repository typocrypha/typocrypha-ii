using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectAddSpellWord : BadgeEffect
{
    [SerializeField] private SpellWord word;

    public override void Equip(Caster player)
    {
        PlayerDataManager.instance.equipment.EquipWord(word);
        SpellCooldownManager.instance.AddWord(word, true);
    }

    public override void Unequip(Caster player)
    {
        PlayerDataManager.instance.equipment.UnequipWord(word);
        SpellCooldownManager.instance.RemoveWord(word);
    }
}
