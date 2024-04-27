using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public interface IATBStateMachine
    {
        PauseHandle PH { get;  }
        ATBStateID CurrentStateID { get; set; }
        IATBState CurrentATBState { get; }

        void PerformTransition(ATBStateID transitionTo);
    }
}
