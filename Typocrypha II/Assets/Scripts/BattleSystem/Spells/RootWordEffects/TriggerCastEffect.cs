using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCastEffect : RootWordEffect
{
    [SerializeField] private string id;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        if(target.Spell == null || target.Stunned || target.Countered || target.IsInactive)
        {
            var missResults = InitializeCastResults(caster, target, mod);
            missResults.Miss = true;
            return missResults;
        }
        var actor = target.GetComponent<ATB3.ATBActor>();
        actor.BaseStateMachine.PerformTransition(ATB3.ATBStateID.Cast);
        return InitializeCastResults(caster, target, mod);
    }
}
