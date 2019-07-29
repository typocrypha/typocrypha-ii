using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_BeforeCast : ATBState<ATBPlayer>
    {
        float timer = 0f;

        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.BeforeCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            ATBManager.Instance.EnterSolo(Owner);
            var caster = Owner.GetComponent<Caster>();
            FaderManager.instance.FadeTargets(caster.Spell, caster.FieldPos, caster.TargetPos);
            Owner.isCast = true;
            timer = 0f;
        }
        
        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= 1f)
                Source.PerformTransition(ATBTransition.ToCast);
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
