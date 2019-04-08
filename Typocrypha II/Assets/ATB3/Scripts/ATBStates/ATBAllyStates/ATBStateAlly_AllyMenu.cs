using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_AllyMenu : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AllyMenu; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            ATBManager.Instance.enterSolo(this.Owner);
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {

        }
    }
}
