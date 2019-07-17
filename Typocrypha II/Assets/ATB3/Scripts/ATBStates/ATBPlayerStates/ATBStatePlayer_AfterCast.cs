using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_AfterCast : ATBState<ATBPlayer>
    {
        float time = 0f;

        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AfterCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the AFTERCAST state!");
            time = 0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            if (time < 1f) time += Time.fixedDeltaTime;
            else
                Source.PerformTransition(ATBTransition.ToIdle);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
            FaderManager.instance.FadeAll(0f, Color.black);
            ATBManager.Instance.exitSolo(this.Owner);
        }
    }
}
