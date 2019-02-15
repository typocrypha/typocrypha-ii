using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Cast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Cast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the CAST state!");
            // this.timePassed = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            Source.PerformTransition(ATBTransition.ToAfterCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the CAST state!");
        }
    }
}
