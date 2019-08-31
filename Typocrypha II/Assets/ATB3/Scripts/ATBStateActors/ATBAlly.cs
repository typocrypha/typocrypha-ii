using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    [RequireComponent(typeof(Caster))]
    [RequireComponent(typeof(ATBStateMachine_Ally))]
    public partial class ATBAlly : ATBActor
    {
        public const float activationWindow = 0.5f;
        public ATBStateMachine_Ally StateMachine { get; private set; }
        public override IATBStateMachine BaseStateMachine => StateMachine;
        public Caster Caster { get; private set; }
        public AllyMenu allyMenu; // Ally menu (for choosing spell).
        public int mpMax;
        public float mpChargeTime;
        public float Mp
        {
            get => mp;
            set
            {
                mp = value;
                Caster.Charge = mp / mpMax;
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
            // return if we are not currently an ally
            if (Caster.CasterState != Caster.State.Ally)
                return;
            // return if we are currently casting, have an open ally menu,
            // We are not currently in solo, or if we don't have enough MP to cast anything
            if (isCast || allyMenu.gameObject.activeSelf || !ATBManager.instance.InSolo || !allyMenu.CanCast)
                return;
            // Calculate ally key
            KeyCode menuKey;
            if (Caster.FieldPos == leftAllyPos)
                menuKey = KeyCode.LeftArrow;
            else if (Caster.FieldPos == rightAllyPos)
                menuKey = KeyCode.RightArrow;
            else
                return;
            Debug.Log("most of the way");
            // Actually test for the key
            if (Input.GetKeyDown(menuKey))
            {
                if (ATBManager.instance.SoloActor.isCurrentState(ATBStateID.BeforeCast))
                    Menu(ATBStateID.BeforeCast);                
                else if (ATBManager.instance.SoloActor.isCurrentState(ATBStateID.AfterCast))
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
            StateMachine.PerformTransition(ATBTransition.ToBeforeCast);
        }
    }
}

