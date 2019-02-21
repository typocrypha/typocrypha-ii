using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_BeforeCast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.BeforeCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ALLY " + this.Owner.actorName + " has ENTERED the BEFORECAST state!");
            // THIS IS WHERE THE ENEMY SHOULD GET SOLO'D
            ATBManager.Instance.enterSolo(this.Owner);
            this.Owner.isCast = true;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            Source.PerformTransition(ATBTransition.ToCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ALLY " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
