using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCallReinforcementsEffect : RootWordEffect
{
    public int number = 1;

    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        Clear();
        CastResults results = new CastResults(caster, target);
        var validPositions = Battlefield.instance.ValidReinforcementPositions;
        results.DisplayDamage = false;
        if (validPositions.Count <= 0 || (RandomUtils.RandomU.instance.RandomDouble() < 0.01 && !tags.Contains("AlwaysHit")))
        {
            results.Miss = true;
            LogMessage("But nobody came!");
            return results;
        }
        results.Miss = false;
        int numReinforcements = System.Math.Min(validPositions.Count, number);
        var data = new List<BattleManager.ReinforcementData>(numReinforcements);
        for (int i = 0; i < numReinforcements; i++)
        {
            // Choose position
            var pos = RandomUtils.RandomU.instance.Choice(validPositions);
            validPositions.Remove(pos);
            // Add Reinforcement
            data.Add(new BattleManager.ReinforcementData(GetReinforcementPrefab(i), pos));           
        }
        if(data.Count > 0)
        {
            BattleManager.instance.StartCoroutine(BattleManager.instance.AddCasters(data));
        }
        return results;
    }

    protected abstract GameObject GetReinforcementPrefab(int reinforcementIndex);
    protected virtual void Clear() { }
}
