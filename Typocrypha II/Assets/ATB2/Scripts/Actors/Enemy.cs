﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{

    //================================================================//
    // ENEMY ACTOR
    // Handles the actions performed by enemies on the battlefield.
    //================================================================//

    public partial class Enemy : Actor
    {
        //----------------------------------------------------------------//
        // UI ELEMENTS                                                    //
        //----------------------------------------------------------------//

        public GameObject chargeUI;

        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        float _charge; // Current amount of time (seconds) spent charging current spell
        public float charge
        {
            get
            {
                return _charge;
            }
            set
            {
                _charge = value;
                chargeUI.GetComponent<ShadowBar>().curr = _charge / chargeTime;
            }
        }
        public float chargeTime; // TESTING: amount of time required to charge currently charging spell

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
            chargeUI.GetComponent<ShadowBar>().reset();
            _charge = 0f;
            while (charge + Time.fixedDeltaTime < chargeTime)
            {
                // Charge while in charge state
                do yield return new WaitForFixedUpdate();
                while (pause || !isCurrentState("Charge"));
                charge += Time.fixedDeltaTime;
            }
            charge = chargeTime;
            //sendEvent("enemyPreCast");
            chargeCRObj = null;
        }

        public override void Setup()
        {
            
        }
    }
}

