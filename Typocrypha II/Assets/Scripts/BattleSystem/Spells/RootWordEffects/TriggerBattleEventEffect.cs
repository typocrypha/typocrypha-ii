using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBattleEventEffect : RootWordEffect
{
    [SerializeField] private string id;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        BattleManager.instance.TriggerBattleEvent(id);
        return InitializeCastResults(caster, target, mod);
    }
}
