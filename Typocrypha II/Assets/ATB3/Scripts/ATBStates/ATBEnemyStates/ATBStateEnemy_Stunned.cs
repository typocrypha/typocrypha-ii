using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Stunned : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Stunned; } }
        private float timer = 0.0f;
        private float stuntime = 5;
        private ATBEnemy enemyOwner;

        // Call upon entering given state
        public override void OnEnter()
        {
            enemyOwner = Owner as ATBEnemy;
            stuntime = enemyOwner.caster.Stats.StaggerTime;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the STUNNED state!");
            enemyOwner.GetComponent<Animator>().SetTrigger("Stun");
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= stuntime || !enemyOwner.caster.Stunned)
                Source.PerformTransition(ATBTransition.ExitStun);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            enemyOwner.caster.Stunned = false;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the STUNNED state!");
        }
    }
}
