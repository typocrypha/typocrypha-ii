using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_Charge : ATBState<ATBAlly>
    {
        // Call upon entering given state
        public override void OnEnter()
        {    
            //Debug.Log("ALLY " + this.Owner.actorName + " has ENTERED the CHARGE state!");
            //Owner.startMana();
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            // Source.PerformTransition(ATBTransition.ChargeFinish);
            //if (Owner.mana >= Owner.maxMana)
            //{
            //    Source.PerformTransition(ATBTransition.ToBeforeCast);
            //}
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ALLY " + this.Owner.actorName + " has EXITED the CHARGE state!");
        }
    }
}
