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

    public partial class ATBEnemy : ATBActor
    {
        public Caster caster; // Associated Caster script.

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
            if (caster.ChargeTime == 0f) caster.ChargeTime = 8f; // DEBUG
            caster.Charge = 0f;
            while (caster.Charge + Time.fixedDeltaTime < caster.ChargeTime)
            {
                // Charge while in charge state
                do yield return new WaitForFixedUpdate();
                while (Pause || !isCurrentState(ATBStateID.Charge));
                caster.Charge += Time.fixedDeltaTime;
            }
            caster.Charge = caster.ChargeTime;
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
            PH.Pause = true;
        }
    }
}

