using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleWordProvider
{
    IReadOnlyList<BattleWord.SequenceData> GetData(string key, out GameObject defaultPrefab);
}
