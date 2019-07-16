using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Charge : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Charge; } }

        private ATBEnemy enemyOwner;

        // Call upon entering given state
        public override void OnEnter()
        {
            enemyOwner = Owner as ATBEnemy;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the CHARGE state! (id: " + StateID.ToString() + ")");
            enemyOwner.GetComponent<Animator>().SetTrigger("Idle");
            enemyOwner.StartCharge();
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            if(enemyOwner.caster.Stunned)
            {
                Source.PerformTransition(ATBTransition.ToStun);
            }
            // Source.PerformTransition(ATBTransition.ChargeFinish);
            if (enemyOwner.caster.Charge >= enemyOwner.caster.ChargeTime)
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
