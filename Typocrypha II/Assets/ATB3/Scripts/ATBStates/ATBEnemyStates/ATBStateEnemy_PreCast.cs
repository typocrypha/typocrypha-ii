using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_PreCast : ATBState<ATBEnemy>
    {
        public const float time = 1;
        public float Timer { get; private set; } = 0.0f;

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the PRECAST state!");
            var caster = Owner.Caster;
            if (caster.Spell.Countered) // Spell is countered
            {
                // Apply callbacks after the whole cast is finished (as if this cast happened)
                caster.OnAfterCastResolved?.Invoke(caster.Spell, caster, false);
                Source.PerformTransition(ATBStateID.Charge);
            }
            else
            {
                Owner.GetComponent<Animator>().SetTrigger("PreCast");

            }
            Timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            var caster = Owner.Caster;
            Timer += Time.fixedDeltaTime * Settings.GameplaySpeed;
            if(caster.BStatus == Caster.BattleStatus.Dead)
            {
                Source.PerformTransition(ATBStateID.Dead);
            }
            else if(caster.BStatus == Caster.BattleStatus.Fled)
            {
                Source.PerformTransition(ATBStateID.Fled);
            }
            else if (caster.Stunned)
            {
                Source.PerformTransition(ATBStateID.Stunned);
            }
            else if (caster.Spell.Countered) // Spell is countered
            {
                // Apply callbacks after the whole cast is finished (as if this cast happened)
                caster.OnAfterCastResolved?.Invoke(caster.Spell, caster, false);
                Source.PerformTransition(ATBStateID.Charge);
            }
            else if (caster.Charge <= 0)
            {
                Source.PerformTransition(ATBStateID.Charge);
            }
            else if (Timer >= time)
            {
                Source.PerformTransition(ATBStateID.Cast);
            }
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the PRECAST state!");
        }
    }
}
