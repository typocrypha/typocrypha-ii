using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBStateEnemy_PreCast : ATBState
    {
        // The ID for this specific ATBState
        protected new ATBStateID stateID = ATBStateID.PreCast;

        public ATBStateEnemy_PreCast()
        {
            Owner = null;
            Source = null;
        }
        public ATBStateEnemy_PreCast(ATBActor actor, ATBStateMachine machine)
        {
            Owner = actor;
            Source = machine;
        }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the PRECAST state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            // Source.PerformTransition(ATBTransition.CastStart);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the PRECAST state!");
        }
    }
}
