using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_PreCast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.PreCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the PRECAST state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            Source.PerformTransition(ATBTransition.ToBeforeCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the PRECAST state!");
        }
    }
}
