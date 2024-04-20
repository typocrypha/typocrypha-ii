using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentWordCasterTagMaxHP", menuName = "Equipment Word/Unique/Caster Tag Max HP")]
public class EquipmentTagMapHPWord : EquipmentTagWord
{
    public override void Equip(Caster player)
    {
        base.Equip(player);
        player.RecalculateMaxHP();
    }

    public override void Unequip(Caster player)
    {
        base.Unequip(player);
        player.RecalculateMaxHP();
    }
}
