using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBPlayer : ATBActor
    {
        void Start()
        {
            Setup();
        }

        public override void Setup()
        {
        }

        // Called when player enters a spell into the cast bar
        public void Cast()
        {
            StateMachine.PerformTransition(ATBTransition.ToBeforeCast);
        }

    }
}

