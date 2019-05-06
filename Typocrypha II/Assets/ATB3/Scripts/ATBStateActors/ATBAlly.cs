using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBAlly : ATBActor
    {
        public Caster caster;
        public KeyCode menuKey; // Key to open ally menu.
        public GameObject allyMenu; // Ally menu (for choosing spell).

        // Incrementally charges 
        IEnumerator ChargeCR()
        {
            if (caster.ChargeTime == 0f) caster.ChargeTime = 5f; // DEBUG
            caster.Charge = 0f;
            while (true)
            {
                do yield return new WaitForFixedUpdate();
                while(Pause || !isCurrentState(ATBStateID.Charge));
                // Charge while in charge state
                if (caster.Charge + Time.fixedDeltaTime < caster.ChargeTime)
                    caster.Charge += Time.fixedDeltaTime;
            }
        }

        void Update()
        {
            if (Pause || isCast) return;
            if (Input.GetKeyDown(menuKey))
            {
                Menu();
            }
        }

        void Start()
        {
            Setup();
        }

        public override void Setup()
        {
            PH.Pause = true;
            StartCoroutine(ChargeCR());
        }

        /// <summary>
        /// Starts ally menu sequence.
        /// </summary>
        public void Menu()
        {
            StateMachine.PerformTransition(ATBTransition.ToAllyMenu);
            allyMenu.SetActive(true);
        }
        
        /// <summary>
        /// Starts cast sequence.
        /// </summary>
        public void Cast()
        {
            StateMachine.PerformTransition(ATBTransition.ToBeforeCast);
        }
    }
}

