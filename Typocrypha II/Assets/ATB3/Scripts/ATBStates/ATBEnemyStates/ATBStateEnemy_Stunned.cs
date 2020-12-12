using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Stunned : ATBState<ATBEnemy>
    {
        private float timer = 0.0f;
        private float stuntime = 5;

        // Call upon entering given state
        public override void OnEnter()
        {
            stuntime = Owner.Caster.Stats.StaggerTime;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the STUNNED state!");
            Owner.GetComponent<Animator>().SetTrigger("Stun");
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (Owner.Caster.BStatus == Caster.BattleStatus.Dead)
            {
                Source.PerformTransition(ATBStateID.Dead);
            }
            else if (timer >= stuntime || !Owner.Caster.Stunned)
            {
                Source.PerformTransition(ATBStateID.PreviousState);
            }
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Owner.Caster.Stunned = false;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the STUNNED state!");
        }
    }
}
