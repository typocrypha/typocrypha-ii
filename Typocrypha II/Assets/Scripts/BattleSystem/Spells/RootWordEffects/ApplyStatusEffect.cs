using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffect : RootWordEffect
{
    public GameObject effectPrefab;
    public override CastResults Cast(Caster caster, Caster target)
    {
        CastResults results = new CastResults(caster, target);
        Instantiate(effectPrefab, target.transform);
        results.DisplayDamage = false;
        return results;
    }
}
