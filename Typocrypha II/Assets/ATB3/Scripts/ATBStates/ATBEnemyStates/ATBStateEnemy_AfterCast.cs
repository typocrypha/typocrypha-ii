using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_AfterCast : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.AfterCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the AFTERCAST state!");
            timer = 0f;
        }

        float timer;

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer > 1.0f)
                Source.PerformTransition(ATBTransition.ToCharge);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
            FaderManager.instance.Solo(Owner.GetComponent<FaderGroup>(), 0.0f, Color.black); // TEMP
            ((ATBEnemy)this.Owner).Caster.Charge = 0.0f;
            ATBManager.Instance.exitSolo(this.Owner);
        }
    }
}
