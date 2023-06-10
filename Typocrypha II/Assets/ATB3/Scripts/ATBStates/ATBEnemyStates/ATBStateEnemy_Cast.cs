using System.Collections;
using System.Linq;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Cast : ATBState<ATBEnemy>
    {
        // Call upon entering given state
        public override void OnEnter()
        {
            var caster = Owner.Caster;
            if (caster.Spell == null)
            {
                Source.PerformTransition(ATBStateID.Charge);
                return;
            }
            var spell = caster.Spell;
            var targetPos = caster.TargetPos;
            bool topLevel = !ATBManager.instance.ProcessingActions;
            Coroutine Cast()
            {
                Owner.GetComponent<Animator>().SetTrigger("Cast");
                return SpellManager.instance.Cast(spell, caster, targetPos);
            }
            ATBManager.instance.QueueSolo(new ATBManager.ATBAction() { Actor = Owner, Action = Cast, OnComplete = CastComplete });
        }

        private void CastComplete()
        {
            if (Owner.Caster.BStatus == Caster.BattleStatus.Dead)
            {
                Source.PerformTransition(ATBStateID.Dead);
            }
            else if (Owner.Caster.BStatus == Caster.BattleStatus.Fled)
            {
                Source.PerformTransition(ATBStateID.Fled);
            }
            else
            {
                Source.PerformTransition(ATBStateID.Charge);
            }
        }

        // Call on fixed update while in given state
        public override void OnUpdate() { }

        // Call upon exiting given state
        public override void OnExit() { }
    }
}
