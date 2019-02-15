using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_Cast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Cast; } }
        private float timer = 0.0f;

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the CAST state!");
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            if (!this.Owner.pause) timer += 1.0f;
            if (timer >= 60.0f && !this.Owner.pause)
                Source.PerformTransition(ATBTransition.ToAfterCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the CAST state!");
        }
    }
}
