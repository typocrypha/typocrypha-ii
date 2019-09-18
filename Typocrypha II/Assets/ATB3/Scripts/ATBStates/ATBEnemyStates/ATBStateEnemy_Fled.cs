using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Fled : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Fled; } }

        // Call upon entering given state
        public override void OnEnter()
        {

        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {

        }

        // Call upon exiting given state
        public override void OnExit()
        {

        }
    }
}
