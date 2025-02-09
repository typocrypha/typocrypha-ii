using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBattleWordEffectFixed : SpawnBattleWordEffect
{
    [SerializeField] private WordData[] data;

    protected override IReadOnlyList<WordData> GetWordData()
    {
        return data;
    }
}
