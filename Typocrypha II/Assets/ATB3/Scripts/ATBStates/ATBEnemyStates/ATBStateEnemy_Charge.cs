using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Charge : ATBState<ATBEnemy>
    {

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the CHARGE state! (id: " + StateID.ToString() + ")");
            Owner.GetComponent<Animator>().SetTrigger("Idle");
            if(Source.PreviousStateID != ATBStateID.Stunned)
            {
                Owner.StartCharge();
            }
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            if (Owner.Caster.BStatus == Caster.BattleStatus.Dead)
                Source.PerformTransition(ATBStateID.Dead);
            if (Owner.Caster.BStatus == Caster.BattleStatus.Fled)
                Source.PerformTransition(ATBStateID.Fled);
            // If stunned, go to stun state
            if (Owner.Caster.Stunned)
            {
                Source.PerformTransition(ATBStateID.Stunned);
            }
            // If finished charging, go to precast
            if (Owner.Caster.Charge >= Owner.Caster.ChargeTime && Owner.Caster.Spell != null)
            {
                Source.PerformTransition(ATBStateID.PreCast);
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
