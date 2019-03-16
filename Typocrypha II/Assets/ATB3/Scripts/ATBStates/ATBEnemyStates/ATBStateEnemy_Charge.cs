using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Charge : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Charge; } }

        // Call upon entering given state
        public override void OnEnter()
        {    
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the CHARGE state! (id: " + StateID.ToString() + ")");
            ((ATBEnemy)this.Owner).startCharge();
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            // Source.PerformTransition(ATBTransition.ChargeFinish);
            if (((ATBEnemy)this.Owner).charge >= ((ATBEnemy)this.Owner).chargeTime)
            {
                Source.PerformTransition(ATBTransition.ToPreCast);
            }
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the CHARGE state!");
        }
    }
}
