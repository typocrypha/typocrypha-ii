using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_PreCast : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.PreCast; } }
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
                Source.PerformTransition(ATBTransition.ToStun);
            }
            if (timer >= 1.0f && !ATBManager.Instance.InSolo)
                Source.PerformTransition(ATBTransition.ToBeforeCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the PRECAST state!");
        }
    }
}
