using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_BeforeCast : ATBState<ATBPlayer>
    {
        float timer = 0f;

        // Call upon entering given state
        public override void OnEnter()
        {
            var caster = Owner.GetComponent<Caster>();
            if (ATBManager.instance.InSolo)
            {
                ATBManager.instance.QueueSolo(Owner);
                SpellManager.instance.QueueCast(caster.Spell, caster, Owner.SavedTargetPos, true);
                Typocrypha.Keyboard.instance.PH.Pause = true;
                Source.PerformTransition(ATBStateID.AfterCast); // skip casting sequence
                return;
            }
            ATBManager.instance.EnterSolo(Owner);
            FaderManager.instance.FadeTargets(caster.Spell, caster.FieldPos, caster.TargetPos);
            Owner.isCast = true;
            timer = 0f;
        }
        
        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (!ATBManager.instance.HasReadyAllies || timer >= ATBAlly.activationWindow)
                Source.PerformTransition(ATBStateID.Cast);
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
