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

        public override void Setup()
        {
            castBar.hidden = false;
            castBar.focus = true;
        }

        // Called when player enters a spell into the cast bar
        public void cast()
        {
            //sendEvent("playerStartCast");
        }

    }
}

