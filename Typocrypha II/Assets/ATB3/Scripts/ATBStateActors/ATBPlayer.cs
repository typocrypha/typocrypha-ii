using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    [RequireComponent(typeof(ATBStateMachine_Player))]
    public partial class ATBPlayer : ATBActor
    {
        public ATBStateMachine_Player StateMachine { get; private set; }
        public override IATBStateMachine BaseStateMachine => StateMachine;
        public Battlefield.Position SavedTargetPos { get; private set; }

        void Awake()
        {
            ph = new PauseHandle(OnPause);
            Setup();
        }

        public override void Setup()
        {
            StateMachine = GetComponent<ATBStateMachine_Player>();
        }

        // Called when player enters a spell into the cast bar
        public void Cast(Battlefield.Position targetPos)
        {
            SavedTargetPos = new Battlefield.Position(targetPos);
            StateMachine.PerformTransition(ATBStateID.BeforeCast);
        }

    }
}

