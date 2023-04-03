using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    [RequireComponent(typeof(Caster))]
    //[RequireComponent(typeof(ATBStateMachine<ATBAlly>))]
    public partial class ATBAlly : ATBActor
    {
        public const float activationWindow = 0.5f;
        public ATBStateMachine<ATBAlly> StateMachine { get; private set; }
        public override IATBStateMachine BaseStateMachine => StateMachine;
        public Caster Caster { get; private set; }
        public AllyMenu allyMenu; // Ally menu (for choosing spell).
        public GameObject menuPrompt;
        public int mpMax;
        public float mpChargeTime;
        public float Mp
        {
            get => mp;
            set
            {
                mp = value;
                Caster.ChargeTime = mpMax;
                Caster.Charge = mp;
            }
        }
        private float mp;

        private static readonly Battlefield.Position leftAllyPos  = new Battlefield.Position(1, 0);
        private static readonly Battlefield.Position rightAllyPos = new Battlefield.Position(1, 2);

        // Incrementally charges 
        IEnumerator ChargeCR()
        {
            Mp = 0f;
            float time = 0f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitWhile(() => Pause || !IsCurrentState(ATBStateID.Charge));
                if (Mp == mpMax)
                    continue;
                time += Time.fixedDeltaTime * Caster.Stats.CastingSpeedMod;
                if(time >= mpChargeTime)
                {
                    ++Mp;
                    time = 0;
                }
            }
        }

        void Update()
        {
            if (allyMenu == null)
                return;
            // return if we are not currently an ally, if we are currently casting, have an open ally menu,
            // We are not currently in solo, or if we don't have enough MP to cast anything
            if (Caster.CasterState != Caster.State.Ally || isCast 
                || allyMenu.gameObject.activeSelf || !ATBManager.instance.InSolo || !allyMenu.CanCast)
            {
                menuPrompt.SetActive(false);
                return;
            }
  
            // Calculate ally key
            KeyCode menuKey;
            if (Caster.FieldPos == leftAllyPos)
                menuKey = KeyCode.LeftArrow;
            else if (Caster.FieldPos == rightAllyPos)
                menuKey = KeyCode.RightArrow;
            else
            {
                menuPrompt.SetActive(false);
                return;
            }
               
            var state = ATBManager.instance.SoloActor.BaseStateMachine.CurrentStateID;
            if (state != ATBStateID.BeforeCast && state != ATBStateID.AfterCast)
            {
                menuPrompt.SetActive(false);
                return;
            }

            menuPrompt.SetActive(true);
            // Actually test for the key
            if (Input.GetKeyDown(menuKey))
            {
                Menu(state);
                StateMachine.PerformTransition(ATBStateID.AllyMenu);
            }
        }

        void Awake()
        {
            ph = new PauseHandle(OnPause);
            Setup();
        }

        public override void Setup()
        {
            StateMachine = GetComponent<ATBStateMachine<ATBAlly>>();
            Caster = GetComponent<Caster>();
            PH.Pause = true;
            StartCoroutine(ChargeCR());
        }

        /// <summary>
        /// Starts ally menu sequence.
        /// </summary>
        public void Menu(ATBStateID state)
        {
            ATBManager.instance.EnterSolo(this);
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
            Caster.TargetPos = Battlefield.instance.Player.TargetPos;
            StateMachine.PerformTransition(ATBStateID.BeforeCast);
        }
        public override void OnPause(bool b)
        {
            base.OnPause(b);
            if(b && menuPrompt != null)
                menuPrompt.SetActive(false);
        }
    }
}

