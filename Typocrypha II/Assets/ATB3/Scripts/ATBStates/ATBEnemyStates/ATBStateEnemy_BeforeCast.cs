using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBStateEnemy_BeforeCast : ATBState
    {
        // The ID for this specific ATBState
        protected new ATBStateID stateID = ATBStateID.BeforeCast;

        public ATBStateEnemy_BeforeCast()
        {
            Owner = null;
            Source = null;
        }
        public ATBStateEnemy_BeforeCast(ATBActor actor, ATBStateMachine machine)
        {
            Owner = actor;
            Source = machine;
        }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the BEFORECAST state!");
            // THIS IS WHERE THE ENEMY SHOULD GET SOLO'D
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            // Source.PerformTransition(ATBTransition.CastPerform);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
