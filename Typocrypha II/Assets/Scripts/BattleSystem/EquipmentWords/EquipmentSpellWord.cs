using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentWordSpell", menuName = "Equipment Word/Spell")]
public class EquipmentSpellWord : EquipmentWord
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
