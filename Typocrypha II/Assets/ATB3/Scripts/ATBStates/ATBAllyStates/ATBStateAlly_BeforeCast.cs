using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_BeforeCast : ATBState<ATBAlly>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.BeforeCast; } }
        private float timer = 0;

        // Call upon entering given state
        public override void OnEnter()
        {
            Owner.isCast = true;
            timer = 0;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= ATBAlly.activationWindow)
                Source.PerformTransition(ATBTransition.ToCast);
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ALLY " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
