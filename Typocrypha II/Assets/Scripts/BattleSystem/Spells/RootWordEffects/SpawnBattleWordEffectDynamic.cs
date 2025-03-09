using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBattleWordEffectDynamic : SpawnBattleWordEffect
{
    [SerializeField] private string key;
    protected override IReadOnlyList<BattleWord.SequenceData> GetSequenceData(Caster caster, out GameObject defaultPrefab)
    {
        var provider = caster.GetComponent<IBattleWordProvider>();
        if (provider == null)
        {
            defaultPrefab = null;
            return System.Array.Empty<BattleWord.SequenceData>();
        }
        return provider.GetData(key, out defaultPrefab);
    }
}
