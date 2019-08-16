using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_BeforeCast : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.BeforeCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            if(Owner.Caster.Spell == null)
            {
                Source.PerformTransition(ATBTransition.ToCharge);
                return;
            }
            FaderManager.instance.FadeTargets(Owner.Caster.Spell, Owner.Caster.FieldPos, Owner.Caster.TargetPos);
            Owner.GetComponent<FaderGroup>().FadeAmount = 0f;
            Owner.GetComponent<Animator>().SetTrigger("BeforeCast");
            ATBManager.Instance.EnterSolo(Owner);
            Owner.isCast = true;
            timer = 0f;
        }

        float timer;

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= 0.8f)
                Source.PerformTransition(ATBTransition.ToCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the BEFORECAST state!");
        }
    }
}
