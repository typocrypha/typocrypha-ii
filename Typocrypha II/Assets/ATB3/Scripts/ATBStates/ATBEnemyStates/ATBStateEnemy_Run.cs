using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Run : ATBState<ATBEnemy>
    {
        private static readonly int runHash = Animator.StringToHash("Run");
        // Call upon entering given state
        public override void OnEnter()
        {
            SetAnimation(runHash);
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
