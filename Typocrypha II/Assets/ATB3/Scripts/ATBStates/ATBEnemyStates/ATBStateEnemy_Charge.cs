using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Charge : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Charge; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the CHARGE state! (id: " + StateID.ToString() + ")");
            Owner.GetComponent<Animator>().SetTrigger("Idle");
            Owner.StartCharge();
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            if (Owner.Caster.BStatus == Caster.BattleStatus.Dead)
                Source.PerformTransition(ATBTransition.ToDeath);
            if (Owner.Caster.BStatus == Caster.BattleStatus.Fled)
                Source.PerformTransition(ATBTransition.ToFlee);
            // If stunned, go to stun state
            if (Owner.Caster.Stunned)
            {
                Source.PerformTransition(ATBTransition.ToStun);
            }
            // If finished charging, go to precast
            if (Owner.Caster.Charge >= Owner.Caster.ChargeTime && Owner.Caster.Spell != null)
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
