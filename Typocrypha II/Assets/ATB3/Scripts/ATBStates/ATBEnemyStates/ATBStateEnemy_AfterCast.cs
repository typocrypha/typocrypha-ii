using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_AfterCast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AfterCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the AFTERCAST state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            ((ATBEnemy)this.Owner).caster.Charge = 0.0f;
            Source.PerformTransition(ATBTransition.ToCharge);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
            // THIS IS WHERE THE ENEMY SHOULD GET NOT SOLO'D
            ATBManager.Instance.exitSolo(this.Owner);
        }
    }
}
