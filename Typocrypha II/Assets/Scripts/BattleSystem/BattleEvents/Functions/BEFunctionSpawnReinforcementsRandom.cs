using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BEFunctionSpawnReinforcementsRandom : BEFunctionSpawnReinforcementsAbstract
{
    [SerializeField] private bool preventRepeats = false;

    private int lastIndex = -1;
    protected override bool Consume => false;

    protected override int GetReinforcementIndex(List<GameObject> reinforcements)
    {
        if (!preventRepeats || lastIndex == -1 || reinforcements.Count == 1)
        {
            return lastIndex = RandomUtils.RandomU.instance.RandomInt(0, reinforcements.Count);
        }
        var options = new List<int>(reinforcements.Count);
        for (int i = 0; i < reinforcements.Count; i++)
        {
            if (i == lastIndex)
                continue;
            options.Add(i);
        }
        return lastIndex = RandomUtils.RandomU.instance.Choice(options);
    }
}
