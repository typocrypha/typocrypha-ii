using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_Cast : ATBState<ATBPlayer>
    {
        // Call upon entering given state
        public override void OnEnter()
        {
            var caster = Owner.GetComponent<Caster>();
            var spell = caster.Spell;
            var targetPos = Owner.SavedTargetPos;
            bool topLevel = !ATBManager.instance.ProcessingActions;
            Coroutine CastFn()
            {
                Owner.isCast = true;
                FaderManager.instance.FadeTargets(spell, caster.FieldPos, targetPos);
                return SpellManager.instance.CastAndCounter(spell, caster, targetPos, null, topLevel);
            }
            ATBManager.instance.InsertSolo(new ATBManager.ATBAction() { Actor = Owner, Action = CastFn, OnComplete = CastComplete });
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {

        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the CAST state!");
        }

        private void CastComplete()
        {
            Source.PerformTransition(ATBStateID.Idle);
        }
    }
}
