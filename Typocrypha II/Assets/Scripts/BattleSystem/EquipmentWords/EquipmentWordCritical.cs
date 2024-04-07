using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentWordCritical", menuName = "Equipment Word/Critical")]
public class EquipmentWordCritical : EquipmentWord
{
    public const float baseCritChance = 0.075f;
    public override void Equip(Caster player)
    {
        player.AddActiveAbilities(Caster.ActiveAbilities.Critical);
    }

    public override void Unequip(Caster player)
    {
        player.RemoveActiveAbilities(Caster.ActiveAbilities.Critical);
    }

    public static bool RollForCritical(Caster player)
    {
        return UnityEngine.Random.Range(0, 1f) <= Mathf.Max(0, baseCritChance + (player.Stats.Luck * 0.015f));
    }
}
