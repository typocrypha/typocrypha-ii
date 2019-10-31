using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_AfterCast : ATBState<ATBAlly>
    {
        private float timer = 0;

        // Call upon entering given state
        public override void OnEnter()
        {
            timer = 0;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= ATBAlly.activationWindow)
                Source.PerformTransition(ATBStateID.Charge);
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            ATBManager.instance.ExitSolo(Owner);
        }
    }
}
