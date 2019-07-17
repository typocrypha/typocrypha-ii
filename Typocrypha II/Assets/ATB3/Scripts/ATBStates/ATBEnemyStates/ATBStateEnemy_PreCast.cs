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
        private ATBEnemy enemyOwner;

        // Call upon entering given state
        public override void OnEnter()
        {
            enemyOwner = Owner as ATBEnemy;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the PRECAST state!");
            enemyOwner.GetComponent<Animator>().SetTrigger("PreCast");
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if(enemyOwner.Caster.Stunned)
            {
                enemyOwner.Caster.Charge = 0;
                var AI = Owner.GetComponent<CasterAI>();
                // DEBUG, REMOVE LATER
                AI.OnAfterCast?.Invoke();
                Source.PerformTransition(ATBTransition.ToStun);
            }
            if (timer >= 1.0f && ATBManager.soloStack.Count == 0)
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
