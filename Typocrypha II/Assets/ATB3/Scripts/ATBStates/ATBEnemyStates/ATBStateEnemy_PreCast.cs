using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_PreCast : ATBState<ATBEnemy>
    {
        private float timer = 0.0f;

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the PRECAST state!");
            Owner.GetComponent<Animator>().SetTrigger("PreCast");
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if(Owner.Caster.Stunned)
            {
                Owner.Caster.Charge = 0;
                // Add lose cast on stun behavior here
                Source.PerformTransition(ATBStateID.Stunned);
            }
            else if(Owner.Caster.Charge <= 0)
            {
                Source.PerformTransition(ATBStateID.Charge);
            }
            if (timer >= 1.0f && !ATBManager.instance.InSolo)
                Source.PerformTransition(ATBStateID.BeforeCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the PRECAST state!");
        }
    }
}
