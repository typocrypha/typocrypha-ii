using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_AfterCast : ATBState<ATBPlayer>
    {
        float timer = 0f;
        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the AFTERCAST state!");
            timer = 0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= ATBAlly.activationWindow)
                Source.PerformTransition(ATBStateID.Idle);
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
            FaderManager.instance.FadeAll(0f, Color.black);
            ATBManager.instance.ExitSolo(Owner);
        }
    }
}
