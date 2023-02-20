using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallReinforcementsEffect : RootWordEffect
{
    public GameObject reinforcementPrefab;
    public int number = 1;

    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target);
        var validPositions = Battlefield.instance.ValidReinforcementPositions;
        results.DisplayDamage = false;
        if (validPositions.Count <= 0 || RandomUtils.RandomU.instance.RandomDouble() < 0.05)
        {
            results.Miss = true;
            LogMessage("But nobody came!");
            return results;
        }
        results.Miss = false;
        int maxReinforcements = validPositions.Count;
        for (int i = 0; i < number && i < maxReinforcements; i++)
        {
            // Choose position
            var pos = RandomUtils.RandomU.instance.Choice(validPositions);
            validPositions.Remove(pos);
            // Spawn reinforcement
            BattleManager.instance.StartCoroutine(BattleManager.instance.AddFieldObject(reinforcementPrefab, pos.Row, pos.Col));
        }
        return results;
    }
}
