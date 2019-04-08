using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_Idle : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Idle; } }

        // Call upon entering given state
        public override void OnEnter()
        {    
            //Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the IDLE state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the Idle state!");
        }
    }
}
