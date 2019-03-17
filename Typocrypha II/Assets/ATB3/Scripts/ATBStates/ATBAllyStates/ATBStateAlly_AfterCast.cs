using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_AfterCast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AfterCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ALLY " + this.Owner.actorName + " has ENTERED the AFTERCAST state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            ((ATBAlly)this.Owner).mana = 0.0f;
            Source.PerformTransition(ATBTransition.ToCharge);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ALLY " + this.Owner.actorName + " has EXITED the AFTERCAST state!");
            // THIS IS WHERE THE ENEMY SHOULD GET NOT SOLO'D
            ATBManager.Instance.exitSolo(this.Owner);
        }
    }
}
