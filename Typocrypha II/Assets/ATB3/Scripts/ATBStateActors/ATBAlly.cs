using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    [RequireComponent(typeof(Caster))]
    [RequireComponent(typeof(ATBStateMachine_Ally))]
    public partial class ATBAlly : ATBActor
    {
        public ATBStateMachine_Ally StateMachine { get; private set; }
        public override IATBStateMachine BaseStateMachine => StateMachine;
        public Caster Caster { get; private set; }
        public KeyCode menuKey; // Key to open ally menu.
        public GameObject allyMenu; // Ally menu (for choosing spell).

        // Incrementally charges 
        IEnumerator ChargeCR()
        {
            if (Caster.ChargeTime == 0f) Caster.ChargeTime = 5f; // DEBUG
            Caster.Charge = 0f;
            while (true)
            {
                do yield return new WaitForFixedUpdate();
                while(Pause || !isCurrentState(ATBStateID.Charge));
                // Charge while in charge state
                if (Caster.Charge + Time.fixedDeltaTime < Caster.ChargeTime)
                    Caster.Charge += Time.fixedDeltaTime;
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
            StateMachine = GetComponent<ATBStateMachine_Ally>();
            Caster = GetComponent<Caster>();
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

