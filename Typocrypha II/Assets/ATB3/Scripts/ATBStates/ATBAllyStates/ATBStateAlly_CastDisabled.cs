using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_CastDisabled : ATBState<ATBAlly>
    {
        // Call upon entering given state
        public override void OnEnter()
        {    
            Debug.Log("ALLY " + Owner.name + " has ENTERED the CASTDISABLED state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ALLY " + Owner.name + " has EXITED the CASTDISABLED state!");
        }
    }
}
