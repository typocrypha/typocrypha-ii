using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BEFunctionSpawnReinforcements : BattleEventFunction
{
    [SerializeField] private bool consume = true;
    [SerializeField] private int number;
    public override void Run()
    {
        Running = true;
        StartCoroutine(SpawnReinforcements());
    }

    protected int GetReinforcementIndex(List<GameObject> reinforcements)
    {
        return 0;
    }

    private IEnumerator SpawnReinforcements()
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
            if (consume)
            {
                reinforcements.RemoveAt(reinforcementIndex);
            }
            // Spawn reinforcement
            yield return StartCoroutine(BattleManager.instance.AddFieldObject(unit, pos.Row, pos.Col, true));
        }
        Running = false;
    }
}
