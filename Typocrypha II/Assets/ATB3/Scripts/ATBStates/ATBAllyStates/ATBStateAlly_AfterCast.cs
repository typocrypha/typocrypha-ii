using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_AfterCast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AfterCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            ((ATBAlly)this.Owner).caster.Charge = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            Source.PerformTransition(ATBTransition.ToCharge);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            ATBManager.Instance.exitSolo(this.Owner);
        }
    }
}
