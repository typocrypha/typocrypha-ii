using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BEFunctionSpawnReinforcementsAbstract : BattleEventFunction
{
    [SerializeField] private int number;
    protected virtual bool Consume => true;
    public override void Run()
    {
        Running = true;
        StartCoroutine(SpawnReinforcements());
    }

    protected virtual int GetReinforcementIndex(List<GameObject> reinforcements)
    {
        return 0;
    }

    protected IEnumerator SpawnReinforcements()
    {
        var field = Battlefield.instance;
        var reinforcements = BattleManager.instance.CurrWave.reinforcementPrefabs;
        var availableSpaces = field.ValidReinforcementPositions;
        foreach (var _ in Enumerable.Range(0, number))
        {
            if (availableSpaces.Count <= 0 || reinforcements.Count <= 0)
                break;
            // Choose position
            var pos = RandomUtils.RandomU.instance.Choice(availableSpaces);
            availableSpaces.Remove(pos);
            // Choose reinforcement
            int reinforcementIndex = GetReinforcementIndex(reinforcements);
            var unit = reinforcements[reinforcementIndex];
            if (Consume)
            {
                reinforcements.RemoveAt(reinforcementIndex);
            }
            // Spawn reinforcement
            yield return StartCoroutine(BattleManager.instance.AddCaster(unit, pos.Row, pos.Col));
        }
        Running = false;
    }
}
