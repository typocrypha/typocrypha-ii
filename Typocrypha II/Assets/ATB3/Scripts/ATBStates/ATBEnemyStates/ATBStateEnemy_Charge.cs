using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBStateEnemy_Charge : ATBState
    {
        // The ID for this specific ATBState
        protected new ATBStateID stateID = ATBStateID.Charge;

        public ATBStateEnemy_Charge()
        {
            Owner = null;
            Source = null;
        }
        public ATBStateEnemy_Charge(ATBActor actor, ATBStateMachine machine)
        {
            Owner = actor;
            Source = machine;
        }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the CHARGE state!");
            ((ATBEnemy)this.Owner).startCharge();
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            // Source.PerformTransition(ATBTransition.ChargeFinish);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the CHARGE state!");
        }
    }
}
