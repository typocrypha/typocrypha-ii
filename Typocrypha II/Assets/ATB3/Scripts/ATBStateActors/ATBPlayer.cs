using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public partial class ATBPlayer : InputCaster
    {
        void Start()
        {
            Setup();
        }

        void Update()
        {
            if (castBar.input.isFocused && castBar.input.text != "" && Input.GetKey(KeyCode.Return))
            {
                cast();
                castBar.input.text = "";
            }
        }

        public override void Setup()
        {
            castBar.hidden = false;
            castBar.focus = true;
        }

        // Called when player enters a spell into the cast bar
        public void cast()
        {
            StateMachine.PerformTransition(ATBTransition.ToBeforeCast);
            //sendEvent("playerStartCast");
        }

    }
}

