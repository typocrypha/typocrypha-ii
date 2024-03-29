﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_AfterCast : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID => ATBStateID.AfterCast;

        // Call upon entering given state
        public override void OnEnter()
        {
            timer = 0f;
        }

        float timer;

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= ATBAlly.activationWindow)
            {
                if (Owner.Caster.BStatus == Caster.BattleStatus.Dead)
                    Source.PerformTransition(ATBTransition.ToDeath);
                else if (Owner.Caster.BStatus == Caster.BattleStatus.Fled)
                    Source.PerformTransition(ATBTransition.ToFlee);
                else
                    Source.PerformTransition(ATBTransition.ToCharge);
            }
                
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            FaderManager.instance.Solo(Owner.GetComponent<FaderGroup>(), 0.0f, Color.black);
            Owner.Caster.Charge = 0.0f;
            ATBManager.instance.ExitSolo(Owner);
        }
    }
}
