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
        public AllyMenu allyMenu; // Ally menu (for choosing spell).
        public int mpMax;
        public float mpChargeTime;
        private float mp;
        public float Mp
        {
            get => mp;
            set
            {
                mp = value;
                Caster.Charge = mp / mpMax;
            }
        }


        // Incrementally charges 
        IEnumerator ChargeCR()
        {
            Mp = 0f;
            float time = 0f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitWhile(() => Pause || !isCurrentState(ATBStateID.Charge));
                if (Mp == mpMax)
                    continue;
                time += Time.fixedDeltaTime;
                if(time >= mpChargeTime)
                {
                    ++Mp;
                    time = 0;
                }
            }
        }

        void Update()
        {
            if (Pause || isCast || !ATBManager.Instance.InSolo)
                return;
            if (Input.GetKeyDown(menuKey) && allyMenu.CanCast)
            {
                if (ATBManager.Instance.SoloActor.isCurrentState(ATBStateID.BeforeCast))
                    Menu(ATBStateID.BeforeCast);
                else if (ATBManager.Instance.SoloActor.isCurrentState(ATBStateID.AfterCast))
                    Menu(ATBStateID.AfterCast);
            }
        }

        void Awake()
        {
            ph = new PauseHandle(OnPause);
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
        public void Menu(ATBStateID state)
        {
            ATBManager.Instance.EnterSolo(this);
            StateMachine.PerformTransition(ATBTransition.ToAllyMenu);
            allyMenu.gameObject.SetActive(true);
            allyMenu.Activate(state);
        }
        
        /// <summary>
        /// Starts cast sequence.
        /// </summary>
        public void Cast(Spell spell)
        {
            Mp -= spell.Cost;
            Caster.Spell = spell;
            StateMachine.PerformTransition(ATBTransition.ToBeforeCast);
        }
    }
}

