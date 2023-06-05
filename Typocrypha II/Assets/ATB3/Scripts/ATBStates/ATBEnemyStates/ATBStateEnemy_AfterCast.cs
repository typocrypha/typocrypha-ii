using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_AfterCast : ATBState<ATBEnemy>
    {
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
            if (!ATBManager.instance.HasReadyAllies || timer >= ATBAlly.activationWindow)
            {
                if (Owner.Caster.BStatus == Caster.BattleStatus.Dead)
                    Source.PerformTransition(ATBStateID.Dead);
                else if (Owner.Caster.BStatus == Caster.BattleStatus.Fled)
                    Source.PerformTransition(ATBStateID.Fled);
                else
                    Source.PerformTransition(ATBStateID.Charge);
            }
                
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            if (Owner.Caster.BStatus != Caster.BattleStatus.Fled) 
            {
                FaderManager.instance.Solo(Owner.GetComponent<FaderGroup>(), 0.0f, Color.black);
                Owner.Caster.Charge = 0.0f;
            }
            else // No pause for run spell
            {
                Owner.Caster.Charge = 0.0f;
            }

        }
    }
}
