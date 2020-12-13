﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_PreCast : ATBState<ATBEnemy>
    {
        private float timer = 0.0f;

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has ENTERED the PRECAST state!");
            var caster = Owner.Caster;
            if (caster.Spell.Countered) // Spell is countered
            {
                // Apply callbacks after the whole cast is finished (as if this cast happened)
                caster.OnAfterCastResolved?.Invoke(caster.Spell, caster);
                Source.PerformTransition(ATBStateID.Charge);
            }
            else
            {
                Owner.GetComponent<Animator>().SetTrigger("PreCast");

            }
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            var caster = Owner.Caster;
            timer += Time.fixedDeltaTime;
            if (caster.Stunned)
            {
                Source.PerformTransition(ATBStateID.Stunned);
            }
            else if (caster.Spell.Countered) // Spell is countered
            {
                // Apply callbacks after the whole cast is finished (as if this cast happened)
                caster.OnAfterCastResolved?.Invoke(caster.Spell, caster);
                Source.PerformTransition(ATBStateID.Charge);
            }
            else if (caster.Charge <= 0)
            {
                Source.PerformTransition(ATBStateID.Charge);
            }
            else if (timer >= 1.0f && !ATBManager.instance.InSolo)
            {
                Source.PerformTransition(ATBStateID.BeforeCast);
            }
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the PRECAST state!");
        }
    }
}
