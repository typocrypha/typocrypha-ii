using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBattleWordEffectFixed : SpawnBattleWordEffect
{
    [SerializeField] private List<BattleWord.SequenceData> sequenceData;
    [SerializeField] private GameObject prefab;
    protected override IReadOnlyList<BattleWord.SequenceData> GetSequenceData(Caster caster, out GameObject defaultPrefab)
    {
        defaultPrefab = prefab;
        return sequenceData;
    }
}
