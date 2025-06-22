using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{

    //================================================================//
    // ENEMY ACTOR
    // Handles the actions performed specifically by enemies 
    // on the battlefield.
    //================================================================//

    [RequireComponent(typeof(Caster))]
    [RequireComponent(typeof(ATBStateMachine_Enemy))]
    public partial class ATBEnemy : ATBActor
    {
        public ATBStateMachine_Enemy StateMachine { get; private set; }
        public override IATBStateMachine BaseStateMachine => StateMachine;
        public Caster Caster { get; private set; } // Associated Caster script.

        //----------------------------------------------------------------//
        // CHARGE COROUTINE                                               //
        //----------------------------------------------------------------//

        Coroutine chargeCRObj; // Coroutine that charges spells

        // Start charging current spell (unless old progress is saved)
        public void StartCharge()
        {
            if (chargeCRObj != null)
            {
                StopCoroutine(chargeCRObj);
            }
            chargeCRObj = StartCoroutine(ChargeCR());
        }

        private bool CanCharge()
        {
            return !IsPausedOrCasting() && IsCurrentState(ATBStateID.Charge);
        }

        // Incrementally charges next spell
        IEnumerator ChargeCR()
        {
            var chargeYielder = new WaitUntil(CanCharge);
            var timeYielder = new WaitForFixedUpdate();
            do
            {
                // Charge while in charge state
                if (!CanCharge())
                {
                    yield return chargeYielder;
                }
                yield return timeYielder;
                Caster.Charge += Time.fixedDeltaTime * Settings.GameplaySpeed * Caster.Stats.CastingSpeedMod;
            }
            while (Caster.Charge < Caster.ChargeTime);
            Caster.Charge = Caster.ChargeTime;
            chargeCRObj = null;
        }

        public override void Setup()
        {
            StateMachine = GetComponent<ATBStateMachine_Enemy>();
            Caster = GetComponent<Caster>();
        }
    }
}

