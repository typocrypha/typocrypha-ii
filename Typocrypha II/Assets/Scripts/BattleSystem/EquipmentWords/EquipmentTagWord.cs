using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentWordCasterTag", menuName = "Equipment Word/Caster Tag")]
public class EquipmentTagWord : EquipmentWord
{
    [SerializeField] private CasterTag tag;

    public override void Equip(Caster player)
    {
        player.AddTag(tag);
    }

    public override void Unequip(Caster player)
    {
        player.RemoveTag(tag);
    }
}
