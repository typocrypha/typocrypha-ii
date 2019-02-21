﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Stunned : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Stunned; } }
        private float timer = 0.0f;

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the STUNNED state!");
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 5.0f)
                Source.PerformTransition(ATBTransition.ExitStun);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the STUNNED state!");
        }
    }
}
