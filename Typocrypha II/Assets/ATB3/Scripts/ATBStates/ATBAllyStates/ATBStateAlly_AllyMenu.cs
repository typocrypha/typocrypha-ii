using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_AllyMenu : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AllyMenu; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ALLY " + this.Owner.actorName + " has ENTERED the ALLYMENU state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ALLY " + this.Owner.actorName + " has EXITED the ALLYMENU state!");
        }
    }
}
