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
            ATBManager.instance.EnterSolo(Owner);
            var caster = Owner.GetComponent<Caster>();
            FaderManager.instance.FadeTargets(caster.Spell, caster.FieldPos, caster.TargetPos);
            Owner.isCast = true;
            timer = 0f;
        }
        
        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= ATBAlly.activationWindow)
                Source.PerformTransition(ATBStateID.Cast);
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
