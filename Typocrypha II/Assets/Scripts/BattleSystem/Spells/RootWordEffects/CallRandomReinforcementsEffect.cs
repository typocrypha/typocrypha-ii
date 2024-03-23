using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallRandomReinforcementsEffect : AbstractCallReinforcementsEffect
{
    public List<GameObject> reinforcementPrefabs;
    public bool preventRepeats = false;

    private readonly List<GameObject> validOptions = new List<GameObject>();

    protected override void Clear()
    {
        if (preventRepeats)
        {
            validOptions.Clear();
            validOptions.AddRange(reinforcementPrefabs);
        }
    }

    protected override GameObject GetReinforcementPrefab(int reinforcementIndex)
    {
        if (!preventRepeats)
        {
            return RandomU.instance.Choice(reinforcementPrefabs);
        }
        if(validOptions.Count <= 0)
        {
            validOptions.AddRange(reinforcementPrefabs);
        }
        var choice = RandomU.instance.Choice(validOptions, out int choiceIndex);
        validOptions.RemoveAt(choiceIndex);
        return choice;
    }
}
