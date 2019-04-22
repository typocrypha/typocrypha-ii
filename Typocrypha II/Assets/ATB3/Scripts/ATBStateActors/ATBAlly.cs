using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBAlly : InputCaster
    {
        // UI Objects
        public GameObject healthUI;
        public GameObject manaUI;
        public GameObject allyMenu; // Ally menu (for choosing spell).

        // Properties
        float _mana; // Current amount of time (seconds) spent charging current spell
        public float mana
        {
            get
            {
                return _mana;
            }
            set
            {
                _mana = value;
                manaUI.GetComponent<ShadowBar>().Curr = _mana / maxMana;
            }
        }
        public float maxMana; // TESTING: max mana
        public float manaRate; // TESTING: rate at which mana is charged (per sec)
        public float manaCost; // TESTING: cost of spell

        Coroutine manaCRObj; // Coroutine that charges mana

        // Start charging mana
        public void startMana()
        {
            manaUI.GetComponent<ShadowBar>().Reset();
            _mana = 0f;
            manaCRObj = StartCoroutine(manaCR());
        }

        // Incrementally charges mana
        IEnumerator manaCR()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                // Cap off mana
                if (mana >= maxMana)
                {
                    mana = maxMana;
                }
                // Charge while in charge state
                else
                {
                    if (!pause && isCurrentState(ATBStateID.Charge))
                        mana += manaRate * Time.fixedDeltaTime;
                }
            }
        }

        void Start()
        {
            Setup();
        }

        public override void Setup()
        {
            startMana();
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

