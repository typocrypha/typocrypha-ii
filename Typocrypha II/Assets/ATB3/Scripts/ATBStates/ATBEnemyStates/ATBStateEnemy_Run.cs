using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Run : ATBState<ATBEnemy>
    {
        public const float time = 0.6f;
        private static readonly int runHash = Animator.StringToHash("Run");
        private float timer = 0.0f;
        // Call upon entering given state
        public override void OnEnter()
        {
            Owner.Caster.BStatus = Caster.BattleStatus.Leaving;
            SetAnimation(runHash);
            timer = 0;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime * Settings.GameplaySpeed;
            if (timer >= time)
            {
                // TODO: not proper functionality, just placeholder to see how effect feels while preventing enemy spawns + wave end
                Owner.Caster.BStatus = Caster.BattleStatus.Fled;
            }
        }

        // Call upon exiting given state
        public override void OnExit()
        {

        }
    }
}
