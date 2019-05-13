using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_BeforeCast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.BeforeCast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the BEFORECAST state!");
            var caster = Owner.GetComponent<Caster>();
            FaderManager.instance.FadeTargets(caster.Spell, caster.FieldPos, caster.TargetPos);
            Owner.GetComponent<FaderGroup>().FadeAmount = 0f;
            ((ATBEnemy)this.Owner).GetComponent<Animator>().SetTrigger("BeforeCast");
            ATBManager.Instance.enterSolo(this.Owner);
            this.Owner.isCast = true;
            timer = 0f;
        }

        float timer;

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 1.0f)
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
