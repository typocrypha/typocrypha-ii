using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBattleEventEffect : RootWordEffect
{
    [SerializeField] private string id;
    public override bool CanCrit => false;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target);
        results.Miss = false;
        results.DisplayDamage = false;
        BattleManager.instance.TriggerBattleEvent(id);
        return results;
    }
}
