using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBPlayer : ATBActor
    {
        public List<SpellWord> currWords = new List<SpellWord>();

        void Start()
        {
            Setup();
        }

        public override void Setup()
        {
        }

        // Called when player enters a spell into the cast bar
        public void cast()
        {
            StateMachine.PerformTransition(ATBTransition.ToBeforeCast);
            //sendEvent("playerStartCast");
        }

    }
}

