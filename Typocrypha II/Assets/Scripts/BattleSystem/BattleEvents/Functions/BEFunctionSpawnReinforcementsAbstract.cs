using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BEFunctionSpawnReinforcementsAbstract : BattleEventFunction
{
    [SerializeField] private int number;
    [SerializeField] private bool avoidLastPosition;
    private Battlefield.Position lastPosition;
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

    protected virtual Battlefield.Position GetReinforcementPosition(IReadOnlyList<Battlefield.Position> validPositions)
    {
        if (validPositions.Count == 1)
            return validPositions[0];
        if (avoidLastPosition)
        {
            var priorityPositions = new List<Battlefield.Position>(validPositions.Count);
            foreach(var pos in validPositions)
            {
                if (pos == lastPosition)
                    continue;
                priorityPositions.Add(pos);
            }
            if(priorityPositions.Count > 0)
            {
                return RandomUtils.RandomU.instance.Choice(priorityPositions);
            }
        }
        return RandomUtils.RandomU.instance.Choice(validPositions);
    }

    protected IEnumerator SpawnReinforcements()
    {
        var field = Battlefield.instance;
        var reinforcements = BattleManager.instance.CurrWave.reinforcementPrefabs;
        var availableSpaces = field.ValidReinforcementPositions;
        var finalPosition = new Battlefield.Position(-1, -1);
        foreach (var _ in Enumerable.Range(0, number))
        {
            if (availableSpaces.Count <= 0 || reinforcements.Count <= 0)
                break;
            // Choose position
            var pos = finalPosition = GetReinforcementPosition(availableSpaces);
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
        if (finalPosition.IsLegal)
        {
            lastPosition = finalPosition;
        }
        Running = false;
    }
}
