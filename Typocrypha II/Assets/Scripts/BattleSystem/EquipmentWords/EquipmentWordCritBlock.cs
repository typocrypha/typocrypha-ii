using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentWordCritBlock", menuName = "Equipment Word/CritBlock")]
public class EquipmentWordCritBlock : EquipmentWord
{
    public const float baseCritChance = 0.075f;
    public override void Equip(Caster player)
    {
        player.AddActiveAbilities(Caster.ActiveAbilities.CriticalBlock);
    }

    public override void Unequip(Caster player)
    {
        player.RemoveActiveAbilities(Caster.ActiveAbilities.CriticalBlock);
    }

    public static bool RollForCritical(Caster player)
    {
        return UnityEngine.Random.Range(0, 1f) <= Mathf.Max(0, baseCritChance + (player.Stats.Luck * 0.015f));
    }
}
