using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_Stunned : ATBState<ATBAlly>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Stunned; } }
        private float timer = 0.0f;
        private float stunTime = 5f;

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ALLY " + Owner.name + " has ENTERED the STUNNED state!");
            timer = 0.0f;
            stunTime = Owner.Caster.Stats.StaggerTime;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= stunTime)
                Source.PerformTransition(ATBTransition.ExitStun);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ALLY " + Owner.name + " has EXITED the STUNNED state!");
        }
    }
}
