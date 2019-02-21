﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_AfterCast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AfterCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the AFTERCAST state!");
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            Source.PerformTransition(ATBTransition.ToIdle);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
            // THIS IS WHERE THE ENEMY SHOULD GET NOT SOLO'D
            ATBManager.Instance.exitSolo(this.Owner);
        }
    }
}
