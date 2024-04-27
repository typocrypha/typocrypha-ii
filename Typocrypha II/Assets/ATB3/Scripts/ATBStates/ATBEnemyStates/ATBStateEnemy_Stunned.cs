using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Stunned : ATBState<ATBEnemy>
    {
        private float timer = 0.0f;
        private float stuntime = 5;

        // Call upon entering given state
        public override void OnEnter()
        {
            stuntime = Owner.Caster.Stats.StaggerTime;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the STUNNED state!");
            Owner.GetComponent<Animator>().SetTrigger("Stun");
            Owner.Caster.StunProgress = 0;
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            var caster = Owner.Caster;
            caster.StunProgress = timer / stuntime;
            if (caster.BStatus == Caster.BattleStatus.Dead)
            {
                Source.PerformTransition(ATBStateID.Dead);
            }
            else if (timer >= stuntime || !Owner.Caster.Stunned)
            {
                // Interrupt if stunned in precast
                if(Source.PreviousStateID == ATBStateID.PreCast || Source.PreviousStateID == ATBStateID.Cast)
                {
                    caster.Charge = 0;
                    caster.OnAfterCastResolved?.Invoke(caster.Spell, caster);
                    Source.PerformTransition(ATBStateID.Charge);
                }
                else
                {
                    Source.PerformTransition(ATBStateID.PreviousState);
                }
            }
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            Owner.Caster.Stunned = false;
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the STUNNED state!");
        }
    }
}
