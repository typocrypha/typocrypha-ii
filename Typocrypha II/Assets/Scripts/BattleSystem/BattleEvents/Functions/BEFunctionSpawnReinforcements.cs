using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BEFunctionSpawnReinforcements : BattleEventFunction
{
    public int number;
    public override void Run()
    {
        StartCoroutine(SpawnReinforcements());
    }

    private IEnumerator SpawnReinforcements()
    {
        var field = Battlefield.instance;
        var reinforcements = BattleManager.instance.CurrWave.reinforcementPrefabs;
        bool ValidPos(Battlefield.Position p)
        {
            if (p.Row != 0)
                return false;
            if (field.GetObject(p) == null)
                return true;
            var caster = field.GetCaster(p);
            return caster.BStatus == Caster.BattleStatus.Dead || caster.BStatus == Caster.BattleStatus.Fled;
        }
        var availableSpaces = field.AllPositions.Where(ValidPos).ToList();
        foreach (var _ in Enumerable.Range(0, number))
        {
            if (availableSpaces.Count <= 0 || reinforcements.Count <= 0)
                break;
            var pos = RandomUtils.RandomU.instance.Choice(availableSpaces);
            availableSpaces.Remove(pos);
            yield return StartCoroutine(BattleManager.instance.AddFieldObject(reinforcements[0], pos.Row, pos.Col, true));
            reinforcements.RemoveAt(0);
        }
    }
}
