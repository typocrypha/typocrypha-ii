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
        public void startCharge()
        {
            if (chargeCRObj == null)
                chargeCRObj = StartCoroutine(chargeCR());   
        }

        // Incrementally charges next spell
        IEnumerator chargeCR()
        {
            if (caster.ChargeTime == 0f) caster.ChargeTime = 10f; // DEBUG
            caster.Charge = 0f;
            while (caster.Charge + Time.fixedDeltaTime < caster.ChargeTime)
            {
                // Charge while in charge state
                //Debug.Log("CHARGING...");
                do yield return new WaitForFixedUpdate();
                while (pause || !isCurrentState(ATBStateID.Charge));
                caster.Charge += Time.fixedDeltaTime;
            }
            caster.Charge = caster.ChargeTime;
            //Debug.Log("DONE CHARGING");
            //sendEvent("enemyPreCast");
            chargeCRObj = null;
        }

        public override void Setup()
        {
            
        }
    }
}

