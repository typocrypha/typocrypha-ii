using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBStateEnemy_Stunned : ATBState
    {
        // The ID for this specific ATBState
        protected new ATBStateID stateID = ATBStateID.Stunned;

        public ATBStateEnemy_Stunned()
        {
            Owner = null;
            Source = null;
        }
        public ATBStateEnemy_Stunned(ATBActor actor, ATBStateMachine machine)
        {
            Owner = actor;
            Source = machine;
        }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the STUNNED state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            // Source.PerformTransition(ATBTransition.ExitStun);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the STUNNED state!");
        }
    }
}
