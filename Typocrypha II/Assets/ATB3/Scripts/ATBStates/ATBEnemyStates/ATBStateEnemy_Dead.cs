using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Dead : ATBState<ATBEnemy>
    {
        private static readonly int deathHash = Animator.StringToHash("Death");
        // Call upon entering given state
        public override void OnEnter()
        {
            SetAnimation(deathHash);
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
