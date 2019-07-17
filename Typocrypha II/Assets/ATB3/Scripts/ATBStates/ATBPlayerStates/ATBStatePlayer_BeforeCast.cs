using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_BeforeCast : ATBState<ATBPlayer>
    {
        float time = 0f;

        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.BeforeCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the BEFORECAST state!");
            // THIS IS WHERE THE PLAYER SHOULD GET SOLO'D
            ATBManager.Instance.enterSolo(this.Owner);
            var caster = Owner.GetComponent<Caster>();
            FaderManager.instance.FadeTargets(caster.Spell, caster.FieldPos, caster.TargetPos);
            this.Owner.isCast = true;
            time = 0f;
        }
        
        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            if (time < 1f) time += Time.fixedDeltaTime;
            else
                Source.PerformTransition(ATBTransition.ToCast);      
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
