using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_CastDisabled : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.CastDisabled; } }

        // Call upon entering given state
        public override void OnEnter()
        {    
            Debug.Log("ALLY " + this.Owner.actorName + " has ENTERED the CASTDISABLED state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ALLY " + this.Owner.actorName + " has EXITED the CASTDISABLED state!");
        }
    }
}
