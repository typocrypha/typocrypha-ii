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
            if (chargeCRObj == null)
                chargeCRObj = StartCoroutine(ChargeCR());   
        }

        // Incrementally charges next spell
        IEnumerator ChargeCR()
        {
            if (Caster.ChargeTime == 0f) Caster.ChargeTime = 8f; // DEBUG
            Caster.Charge = 0f;
            while (Caster.Charge + Time.fixedDeltaTime < Caster.ChargeTime)
            {
                // Charge while in charge state
                do yield return new WaitForFixedUpdate();
                while (Pause || !isCurrentState(ATBStateID.Charge));
                Caster.Charge += Time.fixedDeltaTime;
            }
            Caster.Charge = Caster.ChargeTime;
            //Debug.Log("DONE CHARGING");
            //sendEvent("enemyPreCast");
            chargeCRObj = null;
        }

        void Start()
        {
            Setup();
        }

        public override void Setup()
        {
            StateMachine = GetComponent<ATBStateMachine_Enemy>();
            Caster = GetComponent<Caster>();
            PH.Pause = true;
        }
    }
}

